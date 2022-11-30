using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RoundedRect : Image
{
    public Color fillColor = new Color(1, 1, 1, 1);
    public Color borderColor = new Color(0, 0, 0, 1);
    public Color shadowColor = new Color(0, 0, 0, 0.5f);
    public float borderThickness = 3;
    public float roundedValue = 10;
    public float shadowBlur = 10;
    public Vector2 shadowOffset = new Vector2(5, -5);
    public float shadowScale = 10;
    public bool enableShadow = false;
    public bool enableBorder = true;
    public bool manualShadowScale = false;
    public bool applySpriteToBorder = false;
    
    public enum FillStyle { None, HorizontalForward, HorizontalBackwards, VerticalForward, VerticalBackwards}
    public FillStyle fillStyle;
    
    protected override void Awake()
    {
        material = Resources.Load<Material>("MaterialRoundedRect");
        base.Awake();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        var r = GetPixelAdjustedRect();
        var v = GetVertexPositions(r);

        vh.Clear();

        Vector2 roundedAndBorder = new Vector2(roundedValue, enableBorder ? borderThickness : 0);

        var uvs = GetUVValues(r);
        Vector2 uv1 = new Vector2(uvs.x, uvs.y);
        Vector2 uv2 = new Vector2(uvs.x, uvs.w);
        Vector2 uv3 = new Vector2(uvs.z, uvs.w);
        Vector2 uv4 = new Vector2(uvs.z, uvs.y);

        Vector2 type = new Vector2();
        
        int i = 0;
        
        if (enableShadow)
        {
            float shadowScaleValue = manualShadowScale ? shadowScale : shadowBlur / 2f;

            type = new Vector2(0, 1); //Shadow
            vh.AddVert(new Vector3(v.x - shadowScaleValue + shadowOffset.x, v.y - shadowScaleValue + shadowOffset.y, shadowBlur), shadowColor, uv1, r.size, roundedAndBorder, type, Vector3.zero, Vector4.zero);
            vh.AddVert(new Vector3(v.x - shadowScaleValue + shadowOffset.x, v.w + shadowScaleValue + shadowOffset.y, shadowBlur), shadowColor, uv2, r.size, roundedAndBorder, type, Vector3.zero, Vector4.zero);
            vh.AddVert(new Vector3(v.z + shadowScaleValue + shadowOffset.x, v.w + shadowScaleValue + shadowOffset.y, shadowBlur), shadowColor, uv3, r.size, roundedAndBorder, type, Vector3.zero, Vector4.zero);
            vh.AddVert(new Vector3(v.z + shadowScaleValue + shadowOffset.x, v.y - shadowScaleValue + shadowOffset.y, shadowBlur), shadowColor, uv4, r.size, roundedAndBorder, type, Vector3.zero, Vector4.zero);

            vh.AddTriangle(i + 0, i + 1, i + 2);
            vh.AddTriangle(i + 2, i + 3, i + 0);
            i += 4;
        }

        type = new Vector2(0, 0); //Fill
        vh.AddVert(new Vector3(v.x, v.y), fillColor * color, uv1, r.size, roundedAndBorder, type, new Vector3(1, 0, 0), Vector4.zero);
        vh.AddVert(new Vector3(v.x, v.w), fillColor * color, uv2, r.size, roundedAndBorder, type, new Vector3(0, 1, 0), Vector4.zero);
        vh.AddVert(new Vector3(v.z, v.w), fillColor * color, uv3, r.size, roundedAndBorder, type, new Vector3(0, 1, 1), Vector4.zero);
        vh.AddVert(new Vector3(v.z, v.y), fillColor * color, uv4, r.size, roundedAndBorder, type, new Vector3(0, 0, 1), Vector4.zero);

        vh.AddTriangle(i + 0, i + 1, i + 2);
        vh.AddTriangle(i + 2, i + 3, i + 0);
        i += 4;

        if (enableBorder)
        {
            float borderSpriteStrength = applySpriteToBorder ? 1f : 0f;

            type = new Vector2(1, 0); //Border
            vh.AddVert(new Vector3(v.x, v.y, borderSpriteStrength), borderColor * color, uv1, r.size, roundedAndBorder, type, Vector3.zero, Vector4.zero);
            vh.AddVert(new Vector3(v.x, v.w, borderSpriteStrength), borderColor * color, uv2, r.size, roundedAndBorder, type, Vector3.zero, Vector4.zero);
            vh.AddVert(new Vector3(v.z, v.w, borderSpriteStrength), borderColor * color, uv3, r.size, roundedAndBorder, type, Vector3.zero, Vector4.zero);
            vh.AddVert(new Vector3(v.z, v.y, borderSpriteStrength), borderColor * color, uv4, r.size, roundedAndBorder, type, Vector3.zero, Vector4.zero);

            vh.AddTriangle(i + 0, i + 1, i + 2);
            vh.AddTriangle(i + 2, i + 3, i + 0);
            i += 4;
        }
    }

    private Vector4 GetVertexPositions(Rect r)
    {
        var v = new Vector4(r.x, r.y, r.x + r.width, r.y + r.height);
        float oneMinusFill = 1f - fillAmount;

        if (fillStyle == FillStyle.HorizontalBackwards)
        {
            v.x += r.width * oneMinusFill;
        }
        else if (fillStyle == FillStyle.HorizontalForward)
        {
            v.z -= r.width * oneMinusFill;
        }
        else if (fillStyle == FillStyle.VerticalBackwards)
        {
            v.y += r.height * oneMinusFill;
        }
        else if (fillStyle == FillStyle.VerticalForward)
        {
            v.w -= r.height * oneMinusFill;
        }

        return v;
    }

    private Vector4 GetUVValues(Rect r)
    {
        var uvs = new Vector4(0,0,1,1);
        float oneMinusFill = 1f - fillAmount;

        if (fillStyle == FillStyle.HorizontalBackwards)
        {
            uvs.x = oneMinusFill;
        }
        else if (fillStyle == FillStyle.HorizontalForward)
        {
            uvs.z = fillAmount;
        }
        else if (fillStyle == FillStyle.VerticalBackwards)
        {
            uvs.y = oneMinusFill;
        }
        else if (fillStyle == FillStyle.VerticalForward)
        {
            uvs.w = fillAmount;
        }

        return uvs;
    }

    public override bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        Vector2 pointInRect;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, sp, eventCamera, out pointInRect);
        pointInRect += rectTransform.pivot * rectTransform.sizeDelta;

        if (pointInRect.x < roundedValue && pointInRect.y < roundedValue) //Lower left corner
        {
            return Vector2.Distance(pointInRect, new Vector2(roundedValue, roundedValue)) < roundedValue;
        }
        if (pointInRect.x < roundedValue && pointInRect.y > rectTransform.sizeDelta.y - roundedValue) //Upper left corner
        {
            return Vector2.Distance(pointInRect, new Vector2(roundedValue, rectTransform.sizeDelta.y - roundedValue)) < roundedValue;
        }
        if (pointInRect.x > rectTransform.sizeDelta.x - roundedValue && pointInRect.y > rectTransform.sizeDelta.y - roundedValue) //Upper right corner
        {
            return Vector2.Distance(pointInRect, new Vector2(rectTransform.sizeDelta.x - roundedValue, rectTransform.sizeDelta.y - roundedValue)) < roundedValue;
        }
        if (pointInRect.x > rectTransform.sizeDelta.x - roundedValue && pointInRect.y < roundedValue) //Lower right corner
        {
            return Vector2.Distance(pointInRect, new Vector2(rectTransform.sizeDelta.x - roundedValue, roundedValue)) < roundedValue;
        }
        return true;
    }

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        DefaultValues();
    }
#endif
    private void DefaultValues()
    {
        fillColor = new Color(1, 1, 1, 1);
        borderColor = new Color(0, 0, 0, 1);
        shadowColor = new Color(0, 0, 0, 0.5f);
        borderThickness = 3;
        roundedValue = 10;
        shadowBlur = 10;
        shadowScale = 10;
        shadowOffset = new Vector2(5, -5);
        enableShadow = false;
        enableBorder = true;
        manualShadowScale = false;
        applySpriteToBorder = false;
        material = Resources.Load<Material>("MaterialRoundedRect");
    }
}
