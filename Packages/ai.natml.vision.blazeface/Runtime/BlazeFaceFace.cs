/* 
*   BlazeFace
*   Copyright (c) 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Vision {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public sealed partial class BlazeFacePredictor {

        /// <summary>
        /// Detected face.
        /// </summary>
        public struct Face : IEnumerable<Vector2> {

            #region --Client API--
            /// <summary>
            /// Face rectangle in normalized coordinates.
            /// </summary>
            public Rect rect;

            /// <summary>
            /// Detection confidence score.
            /// </summary>
            public float score;

            /// <summary>
            /// Left eye.
            /// </summary>
            public Vector2 leftEye;

            /// <summary>
            /// Right eye.
            /// </summary>
            public Vector2 rightEye;

            /// <summary>
            /// Nose.
            /// </summary>
            public Vector2 nose;

            /// <summary>
            /// Mouth.
            /// </summary>
            public Vector2 mouth;

            /// <summary>
            /// Left ear.
            /// </summary>
            public Vector2 leftEar;

            /// <summary>
            /// Right ear.
            /// </summary>
            public Vector2 rightEar;
            #endregion


            #region --Operations--

            readonly IEnumerator<Vector2> IEnumerable<Vector2>.GetEnumerator () {
                yield return leftEye;
                yield return rightEye;
                yield return nose;
                yield return mouth;
                yield return leftEar;
                yield return rightEar;
            }

            readonly IEnumerator IEnumerable.GetEnumerator() => (this as IEnumerable<Vector2>).GetEnumerator();
            #endregion
        }
    }
}