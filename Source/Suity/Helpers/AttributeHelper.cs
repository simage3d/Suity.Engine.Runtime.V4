﻿// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Suity.Helpers
{
    public static class AttributeHelper
    {
        /// <summary>
        /// Equals <c>BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic</c>.
        /// </summary>
        public const BindingFlags BindInstanceAll = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static readonly Dictionary<MemberInfo, Attribute[]> customMemberAttribCache = new Dictionary<MemberInfo, Attribute[]>();


#if BRIDGE
        private static Dictionary<Type, Attribute[]> customTypeAttribCache = new Dictionary<Type, Attribute[]>();
        public static IEnumerable<T> GetAttributesCached<T>(this Type type) where T : Attribute
        {
            Attribute[] result;
            if (!customTypeAttribCache.TryGetValue(type, out result))
            {
                result = type.GetCustomAttributes(typeof(T), true).OfType<Attribute>().ToArray();
                customTypeAttribCache[type] = result;
            }

            if (typeof(T) == typeof(Attribute))
                return result as IEnumerable<T>;
            else
                return result.OfType<T>();
        }

        public static T GetAttributeCached<T>(this Type type) where T : Attribute
        {
            return GetAttributesCached<T>(type).FirstOrDefault();
        }
#endif

        /// <summary>
        /// Returns all custom attributes of the specified Type that are attached to the specified member.
        /// Inherites attributes are returned as well. This method is usually faster than <see cref="Attribute.GetCustomAttributes"/>,
        /// because it caches previous results internally.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="member"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetAttributesCached<T>(this MemberInfo member) where T : Attribute
		{
            if (!customMemberAttribCache.TryGetValue(member, out Attribute[] result))
            {
                result = Attribute.GetCustomAttributes(member, true);
                if (member.DeclaringType != null && !member.DeclaringType.IsInterface)
                {
                    IEnumerable<Attribute> query = result;
                    Type[] interfaces = member.DeclaringType.GetInterfaces();
                    if (interfaces.Length > 0)
                    {
                        bool addedAny = false;
                        foreach (Type interfaceType in interfaces)
                        {
#if BRIDGE
                            MemberInfo[] interfaceMembers = interfaceType.GetMember(member.Name, AttributeHelper.BindInstanceAll);
#else
                            MemberInfo[] interfaceMembers = interfaceType.GetMember(member.Name, member.MemberType, AttributeHelper.BindInstanceAll);
#endif
                            foreach (MemberInfo interfaceMemberInfo in interfaceMembers)
                            {
                                IEnumerable<Attribute> subQuery = GetAttributesCached<Attribute>(interfaceMemberInfo);
                                if (subQuery.Any())
                                {
                                    query = query.Concat(subQuery);
                                    addedAny = true;
                                }
                            }
                        }
                        if (addedAny)
                        {
                            result = query.Distinct().ToArray();
                        }
                    }
                }
                customMemberAttribCache[member] = result;
            }

            if (typeof(T) == typeof(Attribute))
				return result as IEnumerable<T>;
			else
				return result.OfType<T>();
		}

        public static T GetAttributeCached<T>(this MemberInfo member) where T : Attribute
        {
            return GetAttributesCached<T>(member).FirstOrDefault();
        }

		/// <summary>
		/// Returns all custom attributes of the specified Type that are attached to the specified member.
		/// Inherites attributes are returned as well. This method is usually faster than <see cref="Attribute.GetCustomAttributes"/>
		/// and similar .Net methods, because it caches previous results internally.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="member"></param>
		/// <returns></returns>
		public static bool HasAttributeCached<T>(this MemberInfo member) where T : Attribute
		{
			return GetAttributesCached<T>(member).Any();
		}
    }
}
