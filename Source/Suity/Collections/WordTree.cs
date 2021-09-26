// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System.Collections.Generic;
using System.Linq;

namespace Suity.Collections
{
    class WordTreeNode
    {
        readonly char _word;
        internal bool _isTerminal;

        Dictionary<char, WordTreeNode> _childNodes;

        public WordTreeNode(char word)
        {
            _word = word;
        }

        public void Add(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return;
            }

            char w = code[0];
            string rest = code.Substring(1, code.Length - 1);

            WordTreeNode childNode = EnsureNode(w);
            if (rest.Length > 0)
            {
                EnsureNode(w).Add(rest);
                childNode.Add(rest);
            }
            else
            {
                childNode._isTerminal = true;
            }
        }
        public bool Match(string str, int index)
        {
            if (str[index] != _word)
            {
                return false;
            }

            if (_isTerminal)
            {
                return true;
            }

            if (_childNodes != null && index + 1 < str.Length)
            {
                return _childNodes.Values.Any(node => node.Match(str, index + 1));
            }

            return false;
        }

        WordTreeNode EnsureNode(char w)
        {
            if (_childNodes == null)
            {
                _childNodes = new Dictionary<char, WordTreeNode>();
            }
            return _childNodes.GetValueOrCreate(w, () => new WordTreeNode(w));
        }
        WordTreeNode GetNode(char w)
        {
            if (_childNodes == null)
            {
                return null;
            }
            return _childNodes.GetValueOrDefault(w);
        }
    }


    public class WordTree
    {
        Dictionary<char, WordTreeNode> _childNodes = new Dictionary<char, WordTreeNode>();

        public void Add(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return;
            }

            char w = code[0];
            string rest = code.Substring(1, code.Length - 1);

            WordTreeNode childNode = EnsureNode(w);
            if (rest.Length > 0)
            {
                EnsureNode(w).Add(rest);
                childNode.Add(rest);
            }
            else
            {
                childNode._isTerminal = true;
            }
        }
        public bool Match(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            for (int i = 0; i < str.Length; i++)
            {
                if (_childNodes.Values.Any(node => node.Match(str, i)))
                {
                    return true;
                }
            }

            return false;
        }

        WordTreeNode EnsureNode(char w)
        {
            if (_childNodes == null)
            {
                _childNodes = new Dictionary<char, WordTreeNode>();
            }
            return _childNodes.GetValueOrCreate(w, () => new WordTreeNode(w));
        }
        WordTreeNode GetNode(char w)
        {
            if (_childNodes == null)
            {
                return null;
            }
            return _childNodes.GetValueOrDefault(w);
        }
    }
}
