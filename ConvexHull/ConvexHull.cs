using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hont
{
    public class ConvexHull : MonoBehaviour
    {
        public Transform[] pointsArray;
        public float height = 4;


        public bool IsInRange(Vector3 comparePoint)
        {
            comparePoint = transform.worldToLocalMatrix.MultiplyPoint3x4(comparePoint);

            var flag = true;
            flag &= comparePoint.y <= height;
            flag &= comparePoint.y >= -height;
            flag &= IsConcaveContain2D(pointsArray, comparePoint);

            if (flag)
                return true;

            return false;
        }

        bool IsConcaveContain2D(Transform[] points, Vector3 compare)
        {
            const float VIRTUAL_RAYCAST_LEN = 100000;

            var comparePoint = (points[1].localPosition + points[0].localPosition) * 0.5f;
            var originPoint = compare;
            comparePoint += (comparePoint - originPoint).normalized * VIRTUAL_RAYCAST_LEN;

            int count = 0;
            for (int i = 0; i < points.Length; i++)
            {
                var a = points[i % points.Length];
                var b = points[(i + 1) % points.Length];

                var r = IsLineSegmentIntersection(a.localPosition, b.localPosition, originPoint, comparePoint);

                if (r) count++;
            }

            return count % 2 == 1;
        }

        bool IsLineSegmentIntersection(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            var crossA = Mathf.Sign(Vector3.Cross(d - c, a - c).y);
            var crossB = Mathf.Sign(Vector3.Cross(d - c, b - c).y);

            if (Mathf.Approximately(crossA, crossB)) return false;

            var crossC = Mathf.Sign(Vector3.Cross(b - a, c - a).y);
            var crossD = Mathf.Sign(Vector3.Cross(b - a, d - a).y);

            if (Mathf.Approximately(crossC, crossD)) return false;

            return true;
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (pointsArray == null) return;

            var cacheMatrix = UnityEditor.Handles.matrix;
            UnityEditor.Handles.matrix = transform.localToWorldMatrix;

            Action drawPoints = () =>
            {
                for (int i = 0; i < pointsArray.Length; i++)
                {
                    var a = pointsArray[i];
                    var b = pointsArray[(i + 1) % pointsArray.Length];

                    if (a == null) continue;
                    if (b == null) continue;

                    var minA = a.localPosition;
                    var minB = b.localPosition;

                    var maxA = a.localPosition;
                    var maxB = b.localPosition;

                    minA.y = -height;
                    minB.y = -height;

                    maxA.y = height;
                    maxB.y = height;

                    UnityEditor.Handles.DrawAAPolyLine(minA, minB);
                    UnityEditor.Handles.DrawAAPolyLine(maxA, maxB);

                    UnityEditor.Handles.DrawAAPolyLine(minA, maxA);
                    UnityEditor.Handles.DrawAAPolyLine(minB, maxB);
                }
            };

            UnityEditor.Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
            drawPoints();
            UnityEditor.Handles.zTest = UnityEngine.Rendering.CompareFunction.GreaterEqual;
            var cacheColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = new Color(0.3f, 0.3f, 0.3f, 0.7f);
            drawPoints();
            UnityEditor.Handles.color = cacheColor;

            UnityEditor.Handles.matrix = cacheMatrix;
        }
#endif
    }
}
