using System.IO;
using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace HayWay.Runtime.Extensions
{
    public enum InterpolationType
    {
        Decastejaul,
        Quadratic,
        Test
    }


    public static class LayerMaskExtesions
    {
        public static bool Contains(this LayerMask layermask, int layer)
        {
            return layermask == (layermask | (1 << layer));
        }
    }
    public static class StringExtenstions
    {
        public static string PathCombine(this string current, params string[] path)
        {
            string combined = Path.Combine(path);
            return Path.Combine(current, combined);
        }
        public static string ToNiceClockFormat(this double timmer)
        {
            int minutes = Mathf.FloorToInt((float)timmer / 60F);
            int seconds = Mathf.FloorToInt((float)timmer - minutes * 60);
            string niceTime = string.Format("{0:00}:{1:00}", minutes, seconds);

            return niceTime;
        }
        /// <summary>
        /// Puts the string into the Clipboard.
        /// </summary>
        public static void CopyToClipboard(this string str)
        {
            GUIUtility.systemCopyBuffer = str;
        }
    }
    public static class VectorExtensions
    {
        public static Vector3 WithY(this Vector3 v, float yValue)
        {
            return new Vector3(v.x, yValue, v.z);
        }
        public static Vector3 WithX(this Vector3 v, float xValue)
        {
            return new Vector3(xValue, v.y, v.z);
        }
        public static Vector3 WithZ(this Vector3 v, float zValue)
        {
            return new Vector3(v.x, v.y, zValue);
        }
        public static Vector3 WithOnlyY(this Vector3 v)
        {
            return new Vector3(0, v.y, 0);
        }
        public static Vector3 WithOnlyX(this Vector3 v)
        {
            return new Vector3(v.x, 0, 0);
        }

        public static float Distance(this Vector3 position, Vector3 other)
        {
            return Vector3.Distance(position, other);
        }

        public static float DistanceWithOnlyX(this Vector3 position, Vector3 other)
        {
            return Vector3.Distance(new Vector3(position.x, 0, 0), new Vector3(other.x, 0, 0));
        }

        public static float DistanceY(this Vector3 position, Vector3 other)
        {
            return Vector3.Distance(position.WithOnlyY(), other.WithOnlyY());
        }

        public static float Distance(this Transform origin, Vector3 destination)
        {
            return Vector3.Distance(origin.position, destination);
        }

        public static float Distance(this Transform origin, Transform destination)
        {
            return Vector3.Distance(origin.position, destination.position);
        }

        public static Vector3 SnapTo(this Vector3 v3, float snapAngle, Vector3 customUpAxis)
        {
            float angle = Vector3.Angle(v3, customUpAxis);
            if (angle < snapAngle / 2.0f)          // Cannot do cross product
                return customUpAxis * v3.magnitude;  //   with angles 0 & 180
            if (angle > 180.0f - snapAngle / 2.0f)
                return -customUpAxis * v3.magnitude;

            float t = Mathf.Round(angle / snapAngle);
            float deltaAngle = (t * snapAngle) - angle;

            Vector3 axis = Vector3.Cross(customUpAxis, v3);
            Quaternion q = Quaternion.AngleAxis(deltaAngle, axis);
            return q * v3;
        }
    }
    public static class FloatExtensions
    {
        public static float GetClampedDistance(this Vector3 from, Vector3 to, float maxDistance, InterpolationType interpolation)
        {


            float curDist = from.Distance(to);
            float ratio = Mathf.Clamp01(curDist / maxDistance);
            float start = 0;
            float center = 0;
            float end = 0;
            float result = 0;

            switch (interpolation)
            {
                case InterpolationType.Decastejaul:

                    center = 0.5f;
                    end = 1;

                    break;
                case InterpolationType.Quadratic:

                    center = 2;
                    end = 0;

                    break;

            }

            //  result = ratio * ratio * start + (1 - ratio) * ratio * center + (1 - ratio) * ratio * center + (1 - ratio) * (1 - ratio) * end;
            result = start * Mathf.Pow(ratio, 2) + 2 * ratio * center * (1 - ratio) + end * Mathf.Pow(1 - ratio, 2);


            return result;

        }


        public enum InterpolationType
        {
            Decastejaul,
            Quadratic,
            Test
        }
    }
    public static class ListExtensions
    {
        private static System.Random rng = new System.Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        
    }
    public static class ColorExetensions
    {
        /// <summary>
        /// 0 a 1
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Color WithAlpha(this Color color, float a)
        {
            Color result = color;
            result.a = a;

            return result;
        }
    }
    public static class MatchExtensions
    {
        /// <summary>
        /// Impar
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsOdd(this int value)
        {
            return value % 2 != 0;

        }
        /// <summary>
        /// Par
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEven(int value)
        {
            return value % 2 == 0;
        }
    }
#if UNITY_EDITOR
    public class GizmoExtensions
    {
        public static void DrawWireCapsule(Vector3 pos, Quaternion rot, float radius, float height, Color color = default(Color))
        {
            if (color != default(Color))
                Handles.color = color;

            Matrix4x4 angleMatrix = Matrix4x4.TRS(pos, rot, Handles.matrix.lossyScale);
            using (new Handles.DrawingScope(angleMatrix))
            {
                var pointOffset = (height - (radius * 2)) / 2;

                //draw sideways
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, radius);
                Handles.DrawLine(new Vector3(0, pointOffset, -radius), new Vector3(0, -pointOffset, -radius));
                Handles.DrawLine(new Vector3(0, pointOffset, radius), new Vector3(0, -pointOffset, radius));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, radius);
                //draw frontways
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, radius);
                Handles.DrawLine(new Vector3(-radius, pointOffset, 0), new Vector3(-radius, -pointOffset, 0));
                Handles.DrawLine(new Vector3(radius, pointOffset, 0), new Vector3(radius, -pointOffset, 0));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, radius);
                //draw center
                Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, radius);
                Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, radius);
            }
        }
    }
#endif

}