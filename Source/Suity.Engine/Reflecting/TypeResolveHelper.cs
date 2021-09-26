﻿// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Suity.Reflecting
{
    public static class TypeResolveHelper
    {
        private static readonly ConcurrentDictionary<string, Type> typeResolveCache = new ConcurrentDictionary<string, Type>();
        private static readonly ConcurrentDictionary<string, MemberInfo> memberResolveCache = new ConcurrentDictionary<string, MemberInfo>();


        public static event EventHandler<ResolveMemberEventArgs> TypeResolve = null;
        /// <summary>
        /// Fired when automatically resolving a certain Member has failed. Allows any subscriber to provide a suitable match.
        /// </summary>
        public static event EventHandler<ResolveMemberEventArgs> MemberResolve = null;


        /// <summary>
        /// Resolves a Type based on its <see cref="GetTypeId">type id</see>.
        /// </summary>
        /// <param name="typeString">The type string to resolve.</param>
        /// <param name="declaringMethod">The generic method that is declaring the Type. Only necessary when resolving a generic methods parameter Type.</param>
        /// <returns></returns>
        public static Type ResolveType(this string typeString, MethodInfo declaringMethod = null)
        {
            return ResolveType(typeString, null, declaringMethod);
        }
        private static Type ResolveType(string typeString, IEnumerable<Assembly> searchAsm, MethodInfo declaringMethod)
        {
            if (typeString == null) return null;

            if (typeResolveCache.TryGetValue(typeString, out Type result)) return result;

            if (searchAsm == null) searchAsm = AppDomain.CurrentDomain.GetAssemblies(); //-.Except(DualityApp.DisposedPlugins).ToArray();
            result = FindType(typeString, searchAsm, declaringMethod);
            if (result != null && declaringMethod == null) typeResolveCache[typeString] = result;

            return result;
        }
        /// <summary>
        /// Resolves a Member based on its <see cref="GetMemberId">member id</see>.
        /// </summary>
        /// <param name="memberString">The <see cref="GetMemberId">member id</see> of the member.</param>
        /// <param name="throwOnError">If true, an Exception is thrown on failure.</param>
        /// <returns></returns>
        public static MemberInfo ResolveMember(string memberString)
        {
            if (memberResolveCache.TryGetValue(memberString, out MemberInfo result)) return result;

            Assembly[] searchAsm = AppDomain.CurrentDomain.GetAssemblies();//-.Except(DualityApp.DisposedPlugins).ToArray();
            result = FindMember(memberString, searchAsm);
            if (result != null) memberResolveCache[memberString] = result;

            return result;
        }



        /// <summary>
        /// Retrieves a Type based on a C# code type string.
        /// </summary>
        /// <param name="csCodeType">The type string to use for the search.</param>
        /// <param name="asmSearch">An enumeration of all Assemblies the searched Type could be located in.</param>
        /// <param name="declaringType">The searched Type's declaring Type.</param>
        /// <returns></returns>
        private static Type FindTypeByCSCode(string csCodeType, IEnumerable<Assembly> asmSearch, Type declaringType = null)
        {
            csCodeType = csCodeType.Trim();

            // Retrieve array ranks
            string[] token = Regex.Split(csCodeType, @"<.+>").Where(s => s.Length > 0).ToArray();
            int arrayRank = 0;
            string elementTypeName = csCodeType;
            if (token.Length > 0)
            {
                MatchCollection arrayMatches = Regex.Matches(token[token.Length - 1], @"\[,*\]");
                if (arrayMatches.Count > 0)
                {
                    string rankStr = arrayMatches[arrayMatches.Count - 1].Value;
                    arrayRank = 1 + rankStr.Count(c => c == ',');
                    elementTypeName = elementTypeName.Substring(0, elementTypeName.Length - rankStr.Length);
                }
            }

            // Handle Arrays
            if (arrayRank > 0)
            {
                Type elementType = FindTypeByCSCode(elementTypeName, asmSearch, declaringType);
                return arrayRank == 1 ? elementType.MakeArrayType() : elementType.MakeArrayType(arrayRank);
            }

            if (csCodeType.IndexOfAny(new[] { '<', '>' }) != -1)
            {
                int first = csCodeType.IndexOf('<');
                int eof = csCodeType.IndexOf('<', first + 1);
                int last = 0;
                while (csCodeType.IndexOf('>', last + 1) > last)
                {
                    int cur = csCodeType.IndexOf('>', last + 1);
                    if (cur < eof || eof == -1) last = cur;
                    else break;
                }
                string[] tokenTemp = new string[3];
                tokenTemp[0] = csCodeType.Substring(0, first);
                tokenTemp[1] = csCodeType.Substring(first + 1, last - (first + 1));
                tokenTemp[2] = csCodeType.Substring(last + 1, csCodeType.Length - (last + 1));
                string[] tokenTemp2 = tokenTemp[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                Type[] types = new Type[tokenTemp2.Length];
                Type mainType = FindTypeByCSCode(tokenTemp[0] + '`' + tokenTemp2.Length, asmSearch, declaringType);
                for (int i = 0; i < tokenTemp2.Length; i++)
                {
                    types[i] = FindTypeByCSCode(tokenTemp2[i], asmSearch);
                }

                // Nested type support
                if (tokenTemp[2].Length > 1 && tokenTemp[2][0] == '.')
                    mainType = FindTypeByCSCode(tokenTemp[2].Substring(1, tokenTemp[2].Length - 1), asmSearch, mainType.MakeGenericType(types));

                if (mainType.IsGenericTypeDefinition)
                {
                    if (declaringType != null)
                        return mainType.MakeGenericType(declaringType.GetGenericArguments().Concat(types).ToArray());
                    else
                        return mainType.MakeGenericType(types);
                }
                else
                    return mainType;
            }
            else
            {
                if (declaringType == null)
                {
                    foreach (Assembly asm in asmSearch)
                    {
                        // Try to retrieve all Types from the current Assembly
                        Type[] types;
                        try { types = asm.GetTypes(); }
                        catch (Exception) { continue; }

                        // Iterate over types and manually compare then
                        foreach (Type t in types)
                        {
                            string nameTemp = t.FullName.Replace('+', '.');
                            if (csCodeType == nameTemp) return t;
                        }
                    }
                }
                else
                {
                    Type[] nestedTypes = declaringType.GetNestedTypes(BindingFlagsPreset.BindStaticAll);
                    foreach (Type t in nestedTypes)
                    {
                        string nameTemp = t.FullName;
                        nameTemp = nameTemp.Remove(0, nameTemp.LastIndexOf('+') + 1);
                        nameTemp = nameTemp.Replace('+', '.');
                        if (csCodeType == nameTemp) return t;
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// Retrieves a Type based on a <see cref="GetTypeId">type id</see> string.
        /// </summary>
        /// <param name="typeName">The type id to use for the search.</param>
        /// <param name="asmSearch">An enumeration of all Assemblies the searched Type could be located in.</param>
        /// <param name="declaringMethod">The generic method that is declaring the Type. Only necessary when resolving a generic methods parameter Type.</param>
        /// <returns></returns>
        private static Type FindType(string typeName, IEnumerable<Assembly> asmSearch, MethodInfo declaringMethod = null)
        {
            typeName = typeName.Trim();
            if (string.IsNullOrEmpty(typeName)) return null;

            // Retrieve generic parameters
            Match genericParamsMatch = Regex.Match(typeName, @"(\[)(\[.+\])(\])");
            string[] genericParams = null;
            if (genericParamsMatch.Groups.Count > 3)
            {
                string genericParamList = genericParamsMatch.Groups[2].Value;
                genericParams = SplitArgs(genericParamList, '[', ']', ',', 0);
                for (int i = 0; i < genericParams.Length; i++)
                    genericParams[i] = genericParams[i].Substring(1, genericParams[i].Length - 2);
            }

            // Process type name
            string[] token = Regex.Split(typeName, @"\[\[.+\]\]");
            string typeNameBase = token[0];
            string elementTypeName = typeName;

            // Handle Reference
            if (token[token.Length - 1].LastOrDefault() == '&')
            {
                Type elementType = ResolveType(typeName.Substring(0, typeName.Length - 1), asmSearch, declaringMethod);
                if (elementType == null) return null;
                return elementType.MakeByRefType();
            }

            // Retrieve array ranks
            int arrayRank = 0;
            MatchCollection arrayMatches = Regex.Matches(token[token.Length - 1], @"\[,*\]");
            if (arrayMatches.Count > 0)
            {
                string rankStr = arrayMatches[arrayMatches.Count - 1].Value;
                arrayRank = 1 + rankStr.Count(c => c == ',');
                elementTypeName = elementTypeName.Substring(0, elementTypeName.Length - rankStr.Length);
            }

            // Handle Arrays
            if (arrayRank > 0)
            {
                Type elementType = ResolveType(elementTypeName, asmSearch, declaringMethod);
                if (elementType == null) return null;
                return arrayRank == 1 ? elementType.MakeArrayType() : elementType.MakeArrayType(arrayRank);
            }

            Type baseType = null;
            // Generic method parameter Types
            if (typeNameBase.StartsWith("``"))
            {
                if (declaringMethod != null)
                {
                    int methodGenArgIndex = int.Parse(typeNameBase.Substring(2, typeNameBase.Length - 2));
                    baseType = declaringMethod.GetGenericArguments()[methodGenArgIndex];
                }
            }
            else
            {
                // Retrieve base type
                foreach (Assembly a in asmSearch)
                {
                    baseType = a.GetType(typeNameBase);
                    if (baseType != null) break;
                }
                // Failed to retrieve base type? Try manually and ignore plus / dot difference.
                if (baseType == null)
                {
                    string assemblyNameGuess = typeName.Split('.', '+').FirstOrDefault();
                    IEnumerable<Assembly> sortedAsmSearch = asmSearch.OrderByDescending(a => a.GetShortAssemblyName() == assemblyNameGuess);
                    foreach (Assembly a in sortedAsmSearch)
                    {
                        // Try to retrieve all Types from the current Assembly
                        Type[] types;
                        try { types = a.GetTypes(); }
                        catch (Exception) { continue; }

                        // Iterate and manually compare names
                        foreach (Type t in types)
                        {
                            if (IsFullTypeNameEqual(typeNameBase, t.FullName))
                            {
                                baseType = t;
                                break;
                            }
                        }
                        if (baseType != null) break;
                    }
                }
                // Failed anyway? Try explicit resolve
                if (baseType == null)
                {
                    ResolveMemberEventArgs args = new ResolveMemberEventArgs(typeNameBase);
                    TypeResolve?.Invoke(null, args);
                    baseType = args.ResolvedMember as Type;
                }
            }

            // Retrieve generic type params
            if (genericParams != null)
            {
                Type[] genericParamTypes = new Type[genericParams.Length];

                for (int i = 0; i < genericParamTypes.Length; i++)
                {
                    // Explicitly referring to generic type definition params: Don't attemp to make it generic.
                    if ((genericParams[i].Length > 0 && genericParams[i][0] == '`') &&
                        (genericParams[i].Length < 2 || genericParams[i][1] != '`')) return baseType;

                    genericParamTypes[i] = ResolveType(genericParams[i], asmSearch, declaringMethod);

                    // Can't find the generic type argument: Fail.
                    if (genericParamTypes[i] == null) return null;
                }

                if (baseType == null)
                    return null;
                else
                    return baseType.MakeGenericType(genericParamTypes);
            }

            return baseType;
        }
        /// <summary>
        /// Retrieves a MemberInfo based on a <see cref="GetMemberId">member id</see>.
        /// </summary>
        /// <param name="typeName">The member string to use for the search.</param>
        /// <param name="asmSearch">An enumeration of all Assemblies the searched Type could be located in.</param>
        /// <returns></returns>
        /// <seealso cref="GetMemberId(MemberInfo)"/>
        private static MemberInfo FindMember(string memberString, IEnumerable<Assembly> asmSearch)
        {
            string[] token = memberString.Split(':');

            Type declaringType = token.Length > 1 ? ResolveType(token[1], asmSearch, null) : null;
            MemberTypes memberType = MemberTypes.Custom;
            if (token.Length > 0)
            {
                switch (token[0][0])
                {
                    case 'T': memberType = MemberTypes.TypeInfo; break;
                    case 'M': memberType = MemberTypes.Method; break;
                    case 'F': memberType = MemberTypes.Field; break;
                    case 'E': memberType = MemberTypes.Event; break;
                    case 'C': memberType = MemberTypes.Constructor; break;
                    case 'P': memberType = MemberTypes.Property; break;
                }
            }

            if (declaringType != null && memberType != MemberTypes.Custom)
            {
                if (memberType == MemberTypes.TypeInfo)
                {
                    return declaringType;
                }
                else if (memberType == MemberTypes.Field)
                {
                    MemberInfo member = declaringType.GetField(token[2], BindingFlagsPreset.BindAll);
                    if (member != null) return member;
                }
                else if (memberType == MemberTypes.Event)
                {
                    MemberInfo member = declaringType.GetEvent(token[2], BindingFlagsPreset.BindAll);
                    if (member != null) return member;
                }
                else
                {
                    int memberParamListStartIndex = token[2].IndexOf('(');
                    int memberParamListEndIndex = token[2].IndexOf(')');
                    string memberParamList = memberParamListStartIndex != -1 ? token[2].Substring(memberParamListStartIndex + 1, memberParamListEndIndex - memberParamListStartIndex - 1) : null;
                    string[] memberParams = SplitArgs(memberParamList, '[', ']', ',', 0);
                    string memberName = memberParamListStartIndex != -1 ? token[2].Substring(0, memberParamListStartIndex) : token[2];
                    Type[] memberParamTypes = memberParams.Select(p => ResolveType(p, asmSearch, null)).ToArray();

                    if (memberType == MemberTypes.Constructor)
                    {
                        ConstructorInfo[] availCtors = declaringType.GetConstructors(memberName == "s" ? BindingFlagsPreset.BindStaticAll : BindingFlagsPreset.BindInstanceAll).Where(
                            m => m.GetParameters().Length == memberParams.Length).ToArray();
                        foreach (ConstructorInfo ctor in availCtors)
                        {
                            bool possibleMatch = true;
                            ParameterInfo[] methodParams = ctor.GetParameters();
                            for (int i = 0; i < methodParams.Length; i++)
                            {
                                string methodParamTypeName = methodParams[i].ParameterType.Name;
                                if (methodParams[i].ParameterType != memberParamTypes[i] && methodParamTypeName != memberParams[i])
                                {
                                    possibleMatch = false;
                                    break;
                                }
                            }
                            if (possibleMatch) return ctor;
                        }
                    }
                    else if (memberType == MemberTypes.Property)
                    {
                        PropertyInfo[] availProps = declaringType.GetProperties(BindingFlagsPreset.BindAll).Where(
                            m => m.Name == memberName &&
                            m.GetIndexParameters().Length == memberParams.Length).ToArray();
                        foreach (PropertyInfo prop in availProps)
                        {
                            bool possibleMatch = true;
                            ParameterInfo[] methodParams = prop.GetIndexParameters();
                            for (int i = 0; i < methodParams.Length; i++)
                            {
                                string methodParamTypeName = methodParams[i].ParameterType.Name;
                                if (methodParams[i].ParameterType != memberParamTypes[i] && methodParamTypeName != memberParams[i])
                                {
                                    possibleMatch = false;
                                    break;
                                }
                            }
                            if (possibleMatch) return prop;
                        }
                    }

                    int genArgTokenStartIndex = token[2].IndexOf("``", StringComparison.Ordinal);
                    int genArgTokenEndIndex = memberParamListStartIndex != -1 ? memberParamListStartIndex : token[2].Length;
                    string genArgToken = genArgTokenStartIndex != -1 ? token[2].Substring(genArgTokenStartIndex + 2, genArgTokenEndIndex - genArgTokenStartIndex - 2) : "";
                    if (genArgTokenStartIndex != -1) memberName = token[2].Substring(0, genArgTokenStartIndex);

                    int genArgListStartIndex = genArgToken.IndexOf('[');
                    int genArgListEndIndex = genArgToken.LastIndexOf(']');
                    string genArgList = genArgListStartIndex != -1 ? genArgToken.Substring(genArgListStartIndex + 1, genArgListEndIndex - genArgListStartIndex - 1) : null;
                    string[] genArgs = SplitArgs(genArgList, '[', ']', ',', 0);
                    for (int i = 0; i < genArgs.Length; i++) genArgs[i] = genArgs[i].Substring(1, genArgs[i].Length - 2);

                    int genArgCount = genArgToken.Length > 0 ? int.Parse(genArgToken.Substring(0, genArgListStartIndex != -1 ? genArgListStartIndex : genArgToken.Length)) : 0;

                    // Select the method that fits
                    MethodInfo[] availMethods = declaringType.GetMethods(BindingFlagsPreset.BindAll).Where(
                        m => m.Name == memberName &&
                        m.GetGenericArguments().Length == genArgCount &&
                        m.GetParameters().Length == memberParams.Length).ToArray();
                    foreach (MethodInfo method in availMethods)
                    {
                        bool possibleMatch = true;
                        ParameterInfo[] methodParams = method.GetParameters();
                        for (int i = 0; i < methodParams.Length; i++)
                        {
                            string methodParamTypeName = methodParams[i].ParameterType.Name;
                            if (methodParams[i].ParameterType != memberParamTypes[i] && methodParamTypeName != memberParams[i])
                            {
                                possibleMatch = false;
                                break;
                            }
                        }
                        if (possibleMatch) return method;
                    }
                }
            }

            // Failed? Try explicit resolve
            ResolveMemberEventArgs args = new ResolveMemberEventArgs(memberString);
            MemberResolve?.Invoke(null, args);
            return args.ResolvedMember;
        }


        /// <summary>
        /// Compares two Type names for equality, ignoring the plus / dot difference.
        /// </summary>
        /// <param name="typeNameA"></param>
        /// <param name="typeNameB"></param>
        /// <returns></returns>
        private static bool IsFullTypeNameEqual(string typeNameA, string typeNameB)
        {
            // Not doing this for performance reasons:
            //string nameTemp = typeNameA.Replace('+', '.');
            //if (typeNameB == nameTemp)

            if (typeNameA.Length != typeNameB.Length) return false;

            for (int i = 0; i < typeNameA.Length; ++i)
            {
                if (typeNameA[i] != typeNameB[i])
                {
                    if (typeNameA[i] == '.' && typeNameB[i] == '+')
                        continue;
                    if (typeNameA[i] == '+' && typeNameB[i] == '.')
                        continue;
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// Performs a selective split operation on the specified string. Intended to be used on hierarchial argument lists.
        /// </summary>
        /// <param name="argList">The argument list to split.</param>
        /// <param name="pushLevel">The char that increases the current hierarchy level.</param>
        /// <param name="popLevel">The char that decreases the current hierarchy level.</param>
        /// <param name="separator">The char that separates two arguments.</param>
        /// <param name="splitLevel">The hierarchy level at which to perform the split operation.</param>
        /// <returns></returns>
        public static string[] SplitArgs(string argList, char pushLevel, char popLevel, char separator, int splitLevel)
        {
            if (argList == null) return new string[0];

            // Splitting the parameter list without destroying generic arguments
            int lastSplitIndex = -1;
            int genArgLevel = 0;
            List<string> ptm = new List<string>();
            for (int i = 0; i < argList.Length; i++)
            {
                if (argList[i] == separator && genArgLevel == splitLevel)
                {
                    ptm.Add(argList.Substring(lastSplitIndex + 1, i - lastSplitIndex - 1));
                    lastSplitIndex = i;
                }
                else if (argList[i] == pushLevel)
                    genArgLevel++;
                else if (argList[i] == popLevel)
                    genArgLevel--;
            }
            ptm.Add(argList.Substring(lastSplitIndex + 1, argList.Length - lastSplitIndex - 1));
            return ptm.ToArray();
        }

        public static void ResetCache()
        {
            typeResolveCache.Clear();
            memberResolveCache.Clear();
        }
    }

    public class ResolveMemberEventArgs : EventArgs
    {
        private readonly string memberId = null;
        private MemberInfo resolvedMember = null;

        /// <summary>
        /// [GET] The Member id to resolve.
        /// </summary>
        public string MemberId
        {
            get { return this.memberId; }
        }
        /// <summary>
        /// [GET / SET] The resolved Member.
        /// </summary>
        public MemberInfo ResolvedMember
        {
            get { return this.resolvedMember; }
            set { this.resolvedMember = value; }
        }

        public ResolveMemberEventArgs(string memberId)
        {
            this.memberId = memberId;
        }
    }
}
