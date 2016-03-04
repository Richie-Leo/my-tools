using System;

using Winista.Text.HtmlParser;
using Winista.Text.HtmlParser.Lex;
using Winista.Text.HtmlParser.Util;

namespace Pandora.Basis.Utils
{
    public class HtmlUtil{
        /// <summary>
        /// Get TR node from a table object.
        /// </summary>
        /// <param name="table">The DOM node of table.</param>
        /// <param name="index">从0开始的位置索引</param>
        /// <returns>TR node, or null if not found.</returns>
        public static ITag GetTr(INode table, int index){
            if(table==null || table.Children==null || table.Children.Count<=0) return null;
            int trIndex = 0;
            for(int i=0; i<table.Children.Count; i++){
                INode node = table.Children[i];
                if(!(node is ITag)) continue;
                ITag tag = node as ITag;
                if(tag.IsEndTag()) continue;
                if(tag.TagName.Trim().ToLower()!="tr") continue;
                if(trIndex < index) {
                    trIndex++;
                    continue;
                }
                return tag;
            }
            return null;
        }

        /// <summary>
        /// Get plain text in the TD node.
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="index">从0开始的位置索引</param>
        /// <returns></returns>
        public static string GetTdText(ITag tr, int index){
            if(tr==null || tr.Children==null || tr.Children.Count<=0) return string.Empty;
            int tdIndex = 0;
            for(int i=0; i<tr.Children.Count; i++){
                INode node = tr.Children[i];
                if(!(node is ITag)) continue;
                ITag tag = node as ITag;
                if(tag.IsEndTag()) continue;
                if(tag.TagName.Trim().ToLower()!="td" && tag.TagName.Trim().ToLower()!="th") continue;
                if(tdIndex < index) {
                    tdIndex++;
                    continue;
                }
                return tag.ToPlainTextString().Trim(' ', '\r', '\n', '\t');
            }
            return string.Empty;
        }
    }
}