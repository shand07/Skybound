using UnityEngine;

namespace Skybound.Characters
{
    public static class FormationUtility
    {
        public static Vector3 GetOffset(
            int index,
            FormationType formationType,
            int columns,
            float spacing)
        {
            if (spacing <= 0f)
                spacing = 1f;

            return formationType switch
            {
                FormationType.Line => GetLineOffset(index, spacing),
                FormationType.Column => GetColumnOffset(index, spacing),
                FormationType.Wedge => GetWedgeOffset(index, spacing),
                _ => GetGridOffset(index, columns, spacing)
            };
        }

        private static Vector3 GetGridOffset(int index, int columns, float spacing)
        {
            if (columns <= 0)
                columns = 1;

            int row = index / columns;
            int column = index % columns;

            float centeredColumn = column - (columns - 1) / 2f;

            return new Vector3(
                centeredColumn * spacing,
                0f,
                row * spacing
            );
        }

        private static Vector3 GetLineOffset(int index, float spacing)
        {
            return new Vector3(index * spacing, 0f, 0f);
        }

        private static Vector3 GetColumnOffset(int index, float spacing)
        {
            return new Vector3(0f, 0f, index * spacing);
        }

        private static Vector3 GetWedgeOffset(int index, float spacing)
        {
            if (index == 0)
                return Vector3.zero;

            int row = (index + 1) / 2;
            int side = index % 2 == 0 ? 1 : -1;

            return new Vector3(
                side * row * spacing,
                0f,
                row * spacing
            );
        }
    }
}