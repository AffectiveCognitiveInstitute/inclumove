// <copyright file=Pyramid.cs/>
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

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Aci.Unity.UserInterface
{
    /// <summary>
    ///     Pyramid to display on user interface.
    /// </summary>
    public class Pyramid : UIBehaviour
    {
        /// <summary>
        ///     Highlight Modes for the blocks.
        /// </summary>
        public enum HighlightModes
        {
            /// <summary>
            ///     Highlight can only be on one block.
            /// </summary>
            ExclusiveNormal,

            /// <summary>
            ///     Highlight can only be on one block, the block and the highlight will also be enlarged
            /// </summary>
            ExclusiveScaled,

            /// <summary>
            ///     Highlight can be on multiple blocks
            /// </summary>
            AdditiveNormal,

            /// <summary>
            ///     Highlight can be on multiple blocks, the blocks and the highlight will also be enlarged
            /// </summary>
            AdditiveScaled
        }

        /// <summary>
        ///     Prefab for a pyramid block.
        /// </summary>
        public GameObject Block;

        /// <summary>
        ///     Prefab for a block highlight.
        /// </summary>
        public GameObject BlockHighlight;

        /// <summary>
        ///     Padding between the pyramid blocks
        /// </summary>
        public float BlockPadding;

        private GameObject[] blocks = new GameObject[0]; //Current blocks

        /// <summary>
        ///     The current Highlightmode.
        /// </summary>
        public HighlightModes HighlightMode;

        private float scale; //scale for all block elements

        /// <summary>
        ///     Margin between the pyramid bounding box and the blocks.
        /// </summary>
        public float SideMargin;

        /// <summary>
        ///     The pyramid size. Changing this rebuilds the pyramid.
        /// </summary>
        public int Size
        {
            get { return transform != null ? transform.childCount : 0; }
            set
            {
                if (value == blocks.Length)
                    return;
                Resize(value);
            }
        }

        /// <summary>
        ///     Toggles highlight for a block.
        /// </summary>
        /// <param name="targetBlock"></param>
        public void ToggleHighlight(int targetBlock)
        {
            RectTransform trans = (RectTransform) blocks[targetBlock]?.transform;
            if (trans == null)
                return;
            GameObject highlight = Instantiate(BlockHighlight);
            highlight.name = "Highlight_" + targetBlock;

            if (HighlightMode < HighlightModes.AdditiveNormal) RemoveAllHighlights();
            highlight.transform.SetParent(trans);
            (highlight.transform as RectTransform).anchoredPosition = new Vector2(0, 0);
            highlight.GetComponent<Image>().color = trans.GetComponent<Image>().color;
            highlight.transform.localScale = new Vector3(scale, scale);
            trans.SetAsLastSibling();
            if (HighlightMode == HighlightModes.AdditiveNormal || HighlightMode == HighlightModes.ExclusiveNormal)
                return;
            trans.localScale = new Vector3(scale, scale) * 1.2f;
        }

        /// <summary>
        ///     Sets the color of target pyramid block.
        /// </summary>
        /// <param name="targetBlock">Target block.</param>
        /// <param name="color">Target color.</param>
        public void SetBlockColor(int targetBlock, Color color)
        {
            GameObject obj = blocks[targetBlock]?.gameObject;
            if (obj == null)
                return;
            obj.GetComponent<Image>().color = color;
            if (obj.transform.childCount == 0)
                return;
            obj.transform.GetChild(0).GetComponent<Image>().color = color;
        }

        /// <summary>
        ///     Resets all blocks color in the pyramid and removes all highlights
        /// </summary>
        public void Reset()
        {
            RectTransform parentTrans = (RectTransform) transform;
            // if new pyramid is smaller simple remove all children
            for (int i = 0; i < transform.childCount; ++i)
            {
                Transform toDestroy = parentTrans.GetChild(0);
                toDestroy.SetParent(null);
                Destroy(toDestroy);
            }

            blocks = new GameObject[0];
        }

        private void RemoveAllHighlights()
        {
            // if new pyramid is smaller simple remove all children
            for (int i = 0; i < blocks.Length; ++i)
            {
                if (blocks[i].transform.childCount == 0)
                    continue;
                Transform toDestroy = blocks[i].transform.GetChild(0);
                toDestroy.SetParent(null);
                Destroy(toDestroy.gameObject);
                blocks[i].transform.localScale = new Vector3(scale, scale);
            }
        }

        /// <summary>
        ///     Resizes the pyramid.
        /// </summary>
        /// <param name="newSize">Target size.</param>
        private void Resize(int newSize)
        {
            //Clear previous blocks and create new blocks
            RectTransform parentTrans = (RectTransform) transform;
            foreach (GameObject obj in blocks)
            {
                obj.transform.SetParent(null);
                Destroy(obj);
            }

            blocks = new GameObject[newSize];
            for (int i = 0; i < newSize; ++i)
            {
                blocks[i] = Instantiate(Block);
                blocks[i].name = "Block_" + i;
                blocks[i].transform.SetParent(parentTrans);
            }

            //get the height of the pyramid
            int columnCount = 0;
            int curElements = 0;
            int lastElements = 0;
            while (curElements < newSize)
            {
                ++columnCount;
                lastElements = columnCount * 2 - 1;
                curElements += lastElements;
            }

            //set blocksize and padding
            float width = parentTrans.rect.width;
            float height = parentTrans.rect.height;
            float blockSize = ((RectTransform) Block.transform).rect.width;
            float blockPadding = BlockPadding;
            int maxBlocks = columnCount * 2 - 1;
            scale = blockSize * maxBlocks + blockPadding * (maxBlocks - 1);
            scale = (width - SideMargin * 2) / scale;
            blockSize *= scale;
            blockPadding *= scale;

            //set block positions
            int curBlock = 0;
            for (int y = 0; y < columnCount; ++y)
            {
                // count of the current row
                int rowCount = (columnCount - y) * 2 - 1;
                // if we have less blocks remaining change them to row count
                rowCount = newSize - curBlock < rowCount ? newSize - curBlock : rowCount;
                // anchor on row, padding by amount of blocks missing
                Vector2 rowAnchor = new Vector2(
                    (int) ((maxBlocks - rowCount) * 0.5f) * (blockSize + blockPadding) + SideMargin,
                    y * (blockPadding + blockSize) + SideMargin + blockSize * 0.5f);
                for (int x = 0; x < rowCount; ++x)
                {
                    RectTransform blockTransform = (RectTransform) blocks[curBlock].transform;
                    Vector2 pos = new Vector2(x * (blockPadding + blockSize) + blockSize * 0.5f, 0);
                    blockTransform.anchoredPosition = pos + rowAnchor;
                    blockTransform.localScale = new Vector3(scale, scale);
                    ++curBlock;
                    // if we have reached the last block stop
                    if (curBlock == newSize)
                        return;
                }
            }
        }
    }
}