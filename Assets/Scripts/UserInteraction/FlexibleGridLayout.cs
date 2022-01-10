using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleGridLayout : LayoutGroup
{
    // Copied from https://www.youtube.com/watch?v=CGsEJToeXmA

    public int Rows = 3;
    public int Columns = 3;
    public Vector2 CellSpacing;

    // debug
    public Vector2 CellSize;

    
    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        Vector2 parentDimensions = new Vector2(rectTransform.rect.width, rectTransform.rect.height);

        float cellWidth = (parentDimensions.x - CellSpacing.x * 2) / Columns;
        float cellHeight = (parentDimensions.y - CellSpacing.y * 2) / Rows;

        CellSize = new Vector2(cellWidth, cellHeight);

        int columnCount = 0;
        int rowCount = 0;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            rowCount = i / Mathf.Max(Columns, 1);
            columnCount = i % Columns;

            var item = rectChildren[i];

            Vector2 pos = new Vector2((CellSize.x + CellSpacing.x) * columnCount, (CellSize.y + CellSpacing.y) * rowCount);

            SetChildAlongAxis(item, 0, pos.x, CellSize.x);
            SetChildAlongAxis(item, 1, pos.y, CellSize.y);
        }
    }

    public override void CalculateLayoutInputVertical()
    {
    }

    public override void SetLayoutHorizontal()
    {
    }

    public override void SetLayoutVertical()
    {
    }
}
