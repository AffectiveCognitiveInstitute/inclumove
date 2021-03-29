// <copyright file=Network.cs/>
// <copyright>
//   Copyright (c) 2018, Affective & Cognitive Institute
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software andassociated documentation files
//   (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify,
//   merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//   OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//   LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
//   IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// <license>MIT License</license>
// <main contributors>
//   Moritz Umfahrer
// </main contributors>
// <co-contributors/>
// <patent information/>
// <date>07/12/2018 05:59</date>

namespace Util
{
    /// <summary>
    ///     Contains utility methods for use with network communication between KoBeLU Core application and Unity GUI.
    /// </summary>
    public class Network
    {
        /// <summary>
        ///     Convert html color string from #AARRGGBB format to #RRGGBBAA.
        /// </summary>
        /// <param name="argbColor">Source ARGB string.</param>
        /// <returns>Converted RGBA string.</returns>
        public static string HtmlColorArgbToRgba(string argbColor)
        {
            return "#" + argbColor.Substring(3, 6) + argbColor.Substring(1, 2);
        }

        /// <summary>
        ///     Convert html color string from #RRGGBBAA format to #AARRGGBB.
        /// </summary>
        /// <param name="rgbaColor">Source RGBA string.</param>
        /// <returns>Converted ARGB string.</returns>
        public static string HtmlColorRgbaToArgb(string rgbaColor)
        {
            return "#" + rgbaColor.Substring(7, 2) + rgbaColor.Substring(1, 6);
        }
    }
}