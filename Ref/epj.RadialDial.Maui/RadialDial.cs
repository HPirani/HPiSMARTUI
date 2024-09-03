using Microsoft.Maui.Controls;

using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace epj.RadialDial.Maui;

public sealed class RadialDial : SKCanvasView
{
    #region Constants

    private const float DefaultStartAngle = -90.0f;
    //BeginHPi
    private const float DefaultBaseStartAngle = -90.0f;
    private const float DefaultBaseSweepAngle = 360.0f;
    private const float DefaultMaxSweepAngle = 360.0f;
    private const int DefaultDialScale = 0;//No Scale Our Radial Dial's Size was Be Half of Our Rect.
    private const string DefaultStrokeCap = "Butt";
    private const string DefaultDialStyle = "Dash";
    private  float[] dashArray = new float[6];
    private const float DefaultDashPhase = 2;
    //Morph Style
    private const float Default1DPhase = 0;
    private const float Default1DAdvance = 55;
    //
    private bool pathEffectflag = true;//TODO: Remove This.
    //EndHPi
    #endregion

    #region Private Fields

    private int _size;
    private SKCanvas _canvas;
    private SKRect _dialRect;
    private SKPoint _dialCenter;
    private SKRect _scaleRect;
    private SKPoint _scaleCenter;
    private SKImageInfo _info;
    private SKPoint _touchPoint;
    private bool _hasTouch;
    //BeginHPi
    SKPathEffect defaultPaintStyle;// TODO: Remove This.

    SKPathEffect morphPathEffect;
    //EndHPi
    #endregion

    #region Properties

    public float InternalPadding
    {
        get => (float)GetValue(InternalPaddingProperty);
        set => SetValue(InternalPaddingProperty, value);
    }
    
    //Begin HPi
    public float StartAngle
    {
        get => (float)GetValue(StartAngleProperty);
        set => SetValue(StartAngleProperty, value);
    }
    public float BaseStartAngle
    {
        get => (float)GetValue(BaseStartAngleProperty);
        set => SetValue(BaseStartAngleProperty, value);
    }
    public float BaseSweepAngle
    {
        get => (float)GetValue(BaseSweepAngleProperty);
        set => SetValue(BaseSweepAngleProperty, value);
    }
    public float MaxSweepAngle
    {
        get => (float)GetValue(MaxSweepAngleProperty);
        set => SetValue(MaxSweepAngleProperty, value);
    }
    public int DialSize
    {
        get => (int)GetValue(DialSizeProperty);
        set => SetValue(DialSizeProperty, value);
    }
    public float DashPhase
    {
        get => (float)GetValue(DashPhaseProperty);
        set => SetValue(DashPhaseProperty, value);
    }

    public float Phase1D
    {
        get => (float)GetValue(Phase1DProperty);
        set => SetValue(Phase1DProperty, value);
    }
    public float Advance1D
    {
        get => (float)GetValue(Advance1DProperty);
        set => SetValue(Advance1DProperty, value);
    }
    public string StrokeCap
    {
        get => (string)GetValue(StrokeCapProperty);
        set => SetValue(StrokeCapProperty, value);
    }
    public string DialStyle
    {
        get => (string)GetValue(DialStyleProperty);
        set => SetValue(DialStyleProperty, value);
    }
    //EndHPi
    public float DialWidth
    {
        get => (float)GetValue(DialWidthProperty);
        set => SetValue(DialWidthProperty, value);
    }

    public int Min
    {
        get => (int)GetValue(MinProperty);
        set => SetValue(MinProperty, value);
    }
    public int Max
    {
        get => (int)GetValue(MaxProperty);
        set => SetValue(MaxProperty, value);
    }

    public float Value
    {
        get => (float)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public bool SnapToNearestInteger
    {
        get => (bool)GetValue(SnapToNearestIntegerProperty);
        set => SetValue(SnapToNearestIntegerProperty, value);
    }

    public bool ShowScale
    {
        get => (bool)GetValue(ShowScaleProperty);
        set => SetValue(ShowScaleProperty, value);
    }

    public int ScaleUnits
    {
        get => (int)GetValue(ScaleUnitsProperty);
        set => SetValue(ScaleUnitsProperty, value);
    }

    public float ScaleDistance
    {
        get => (float)GetValue(ScaleDistanceProperty);
        set => SetValue(ScaleDistanceProperty, value);
    }

    public float ScaleLength
    {
        get => (float)GetValue(ScaleLengthProperty);
        set => SetValue(ScaleLengthProperty, value);
    }

    public float ScaleThickness
    {
        get => (float)GetValue(ScaleThicknessProperty);
        set => SetValue(ScaleThicknessProperty, value);
    }

    public Color DialColor
    {
        get => (Color)GetValue(DialColorProperty);
        set => SetValue(DialColorProperty, value);
    }

    public Color BaseColor
    {
        get => (Color)GetValue(BaseColorProperty);
        set => SetValue(BaseColorProperty, value);
    }

    public Color ScaleColor
    {
        get => (Color)GetValue(ScaleColorProperty);
        set => SetValue(ScaleColorProperty, value);
    }

    public bool UseGradient
    {
        get => (bool)GetValue(UseGradientProperty);
        set => SetValue(UseGradientProperty, value);
    }

    public List<Color> GradientColors { get; set; } = new();

    public bool TouchInputEnabled
    {
        get => (bool)GetValue(TouchInputEnabledProperty);
        set => SetValue(TouchInputEnabledProperty, value);
    }

    #endregion

    #region Bindable Properties
    //Begin HPi
    public static readonly BindableProperty StartAngleProperty = BindableProperty.Create(nameof(StartAngle), typeof(float), typeof(RadialDial), DefaultStartAngle, propertyChanged: OnBindablePropertyChanged);
    public static readonly BindableProperty BaseStartAngleProperty = BindableProperty.Create(nameof(BaseStartAngle), typeof(float), typeof(RadialDial), DefaultBaseStartAngle, propertyChanged: OnBindablePropertyChanged);
    public static readonly BindableProperty BaseSweepAngleProperty = BindableProperty.Create(nameof(BaseSweepAngle), typeof(float), typeof(RadialDial), DefaultBaseSweepAngle, propertyChanged: OnBindablePropertyChanged);
    public static readonly BindableProperty MaxSweepAngleProperty = BindableProperty.Create(nameof(MaxSweepAngle), typeof(float), typeof(RadialDial), DefaultMaxSweepAngle, propertyChanged: OnBindablePropertyChanged);
    public static readonly BindableProperty DialSizeProperty = BindableProperty.Create(nameof(DialSize), typeof(int), typeof(RadialDial), DefaultDialScale, propertyChanged: OnBindablePropertyChanged);
    public static readonly BindableProperty StrokeCapProperty = BindableProperty.Create(nameof(StrokeCap), typeof(string), typeof(RadialDial), DefaultStrokeCap, propertyChanged: OnBindablePropertyChanged);
    public static readonly BindableProperty DialStyleProperty = BindableProperty.Create(nameof(DialStyle), typeof(string), typeof(RadialDial), DefaultDialStyle, propertyChanged: OnBindablePropertyChanged);
    public static readonly BindableProperty DashPhaseProperty = BindableProperty.Create(nameof(DashPhase), typeof(float), typeof(RadialDial), DefaultDashPhase, propertyChanged: OnBindablePropertyChanged);
    public static readonly BindableProperty Phase1DProperty = BindableProperty.Create(nameof(Phase1D), typeof(float), typeof(RadialDial), Default1DPhase, propertyChanged: OnBindablePropertyChanged);
    public static readonly BindableProperty Advance1DProperty = BindableProperty.Create(nameof(Advance1D), typeof(float), typeof(RadialDial), Default1DAdvance, propertyChanged: OnBindablePropertyChanged);
   

    //EndHPi
    public static readonly BindableProperty InternalPaddingProperty = BindableProperty.Create(nameof(InternalPadding), typeof(float), typeof(RadialDial), 20.0f, propertyChanged: OnBindablePropertyChanged);

    public static readonly BindableProperty DialWidthProperty = BindableProperty.Create(nameof(DialWidth), typeof(float), typeof(RadialDial), 200.0f, propertyChanged: OnBindablePropertyChanged);

    public static readonly BindableProperty MinProperty = BindableProperty.Create(nameof(Min), typeof(int), typeof(RadialDial), 0, propertyChanged: OnBindablePropertyChanged);

    public static readonly BindableProperty MaxProperty = BindableProperty.Create(nameof(Max), typeof(int), typeof(RadialDial), 60, propertyChanged: OnBindablePropertyChanged);

    public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(float), typeof(RadialDial), 10.0f, BindingMode.TwoWay, propertyChanged: OnBindablePropertyChanged);

    public static readonly BindableProperty SnapToNearestIntegerProperty = BindableProperty.Create(nameof(SnapToNearestInteger), typeof(bool), typeof(RadialDial), true);

    public static readonly BindableProperty DialColorProperty = BindableProperty.Create(nameof(DialColor), typeof(Color), typeof(RadialDial), Colors.Red, propertyChanged: OnBindablePropertyChanged);

    public static readonly BindableProperty BaseColorProperty = BindableProperty.Create(nameof(BaseColor), typeof(Color), typeof(RadialDial), Colors.LightGray, propertyChanged: OnBindablePropertyChanged);

    public static readonly BindableProperty ScaleColorProperty = BindableProperty.Create(nameof(ScaleColor), typeof(Color), typeof(RadialDial), Colors.LightGray, propertyChanged: OnBindablePropertyChanged);

    public static readonly BindableProperty ShowScaleProperty = BindableProperty.Create(nameof(ShowScale), typeof(bool), typeof(RadialDial), true, propertyChanged: OnBindablePropertyChanged);

    public static readonly BindableProperty ScaleUnitsProperty = BindableProperty.Create(nameof(ScaleUnits), typeof(int), typeof(RadialDial), 5, propertyChanged: OnBindablePropertyChanged);

    public static readonly BindableProperty ScaleDistanceProperty = BindableProperty.Create(nameof(ScaleDistance), typeof(float), typeof(RadialDial), 20.0f, propertyChanged: OnBindablePropertyChanged);

    public static readonly BindableProperty ScaleLengthProperty = BindableProperty.Create(nameof(ScaleLength), typeof(float), typeof(RadialDial), 30.0f, propertyChanged: OnBindablePropertyChanged);

    public static readonly BindableProperty ScaleThicknessProperty = BindableProperty.Create(nameof(ScaleThickness), typeof(float), typeof(RadialDial), 10.0f, propertyChanged: OnBindablePropertyChanged);

    public static readonly BindableProperty UseGradientProperty = BindableProperty.Create(nameof(UseGradient), typeof(bool), typeof(RadialDial), false, propertyChanged: OnBindablePropertyChanged);

    public static readonly BindableProperty TouchInputEnabledProperty = BindableProperty.Create(nameof(TouchInputEnabled), typeof(bool), typeof(RadialDial), false, propertyChanged: OnTouchInputEnabledPropertyChanged);

    #endregion

    #region Constructor

    public RadialDial()
    {
        EnableTouchEvents = TouchInputEnabled;
        _hasTouch = false;
    }

    #endregion

    #region Implementation

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        

        base.OnPaintSurface(e);

        _canvas = e.Surface.Canvas;
        _canvas.Clear();

        _info = e.Info;
        _size = (Math.Min(_info.Size.Width, _info.Size.Height)) + DialSize;

        //offsets are used to always center the dial inside the canvas and move the stroke inwards only
        var scaleOffset = InternalPadding;
        var dialOffset = DialWidth / 2 + InternalPadding + ScaleLength + ScaleDistance;

        //setup the drawing rectangle and center for the scale
        _scaleRect = new SKRect(scaleOffset, scaleOffset, _size - scaleOffset, _size - scaleOffset);
        _scaleCenter = new SKPoint(_scaleRect.MidX, _scaleRect.MidY);

        //setup the drawing rectangle and center for the dial
        _dialRect = new SKRect(dialOffset, dialOffset, _size - dialOffset, _size - dialOffset);
        _dialCenter = new SKPoint(_dialRect.MidX, _dialRect.MidY);



        //SKPath.ParseSvgPathData("M -20 -10 L 25 -10, 25 10, -25 10 Z")
        morphPathEffect =
        SKPathEffect.Create1DPath(SKPath.ParseSvgPathData("M -25 -10 L 25 -10, 25 10, -25 10 Z"),
                                   Advance1D, Phase1D, SKPath1DPathEffectStyle.Morph);

        DrawScale();
        DrawBase();
        DrawDial();
    }

    private void DrawScale()
    {
        if (!ShowScale)
        {
            return;
        }

        //calculate amount and divisor for scale units
        var scaleDivisor = (Max - Min) / (float)ScaleUnits;
        var elementCount = (int)Math.Floor(scaleDivisor);

        //we may be able to squeeze in one more scale element
        if ((scaleDivisor - elementCount) * ScaleUnits > 1.0f)
        {
            elementCount += 1;
        }

        //account for scale fractions
        var clipFactor = elementCount / scaleDivisor;
        var clippedAngle = MaxSweepAngle * clipFactor;

        //calculate angles for scale
        var angles = new float[elementCount];
        for (var i = 0; i < elementCount; i++)
        {
            angles[i] = clippedAngle / elementCount * i;
        }

        //rotate canvas before drawing scale element
        _canvas.Save();
        _canvas.RotateDegrees(StartAngle, _dialCenter.X, _dialCenter.Y);

        //draw scale elements for each angle
        foreach (var angle in angles)
        {
            var rad = angle.DegreeToRadian();
            var p0 = rad.ToPointOnCircle(_scaleCenter, _scaleRect.Width / 2);
            var p1 = rad.ToPointOnCircle(_scaleCenter, _scaleRect.Width / 2 - ScaleLength);

            using var path = new SKPath();
            path.AddPoly(new[] { p0, p1 }, close: false);

            _canvas.DrawPath(path, new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = ScaleColor.ToSKColor(),
                StrokeWidth = ScaleThickness,
                IsAntialias = true
            });
        }

        //rotate canvas back to original position
        _canvas.Restore();
    }

    private void DrawDial()
    {
        float sweepAngle;
        var deltaMaxMin = Max - Min;

        if (_hasTouch)
        {
            //calculate the angle of the touch input
            var touchAngle = _touchPoint
                .MapToCircle(_dialCenter, _dialRect.Width / 2)
                .ToAngle(_dialCenter);

            sweepAngle = (touchAngle + StartAngle).NormalizeAngleTo360();

            var resultValue = Min + (deltaMaxMin / MaxSweepAngle * sweepAngle);

            if (SnapToNearestInteger)
            {
                //round to nearest integer and update sweepAngle
                var snapValue = (float)Math.Round(resultValue);
                sweepAngle = MaxSweepAngle / deltaMaxMin * (snapValue - Min);
                Value = snapValue;
            }
            else
            {
                Value = resultValue;
            }
        }
        else
        {
            sweepAngle = MaxSweepAngle / deltaMaxMin * (Value - Min);
        }


        using var dialPath = new SKPath();
        dialPath.AddArc(_dialRect, StartAngle, sweepAngle);
        dashArray[0] = 1 * DialWidth;
        dashArray[1] = 1.5f * DialWidth;
        dashArray[2] = 1 * DialWidth;
        dashArray[3] = 1.5f * DialWidth;
        dashArray[4] = 1 * DialWidth;
        dashArray[5] = 1.5f * DialWidth;
        //  SKStrokeCap cap = SKStrokeCap.Butt;

        Enum.TryParse<SKStrokeCap>(StrokeCap, out SKStrokeCap cap);


        using var dialPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = DialColor.ToSKColor(),
            StrokeWidth = DialWidth,
            StrokeCap = cap,
            
         //   PathEffect = SKPathEffect.CreateCorner(2),
           // PathEffect = 
        IsAntialias = true
        };

        

        if (pathEffectflag)
        {

             defaultPaintStyle = dialPaint.PathEffect;
            pathEffectflag = false;
        }
        switch (DialStyle)
        {

            case "1DMorph":
                dialPaint.PathEffect = morphPathEffect;
                break;
            case "Dash":
                dialPaint.PathEffect = SKPathEffect.CreateDash(dashArray, DashPhase);
                break;
            case "none":  
                dialPaint.PathEffect = defaultPaintStyle;
                break;
        }

        if (UseGradient && GradientColors?.Count > 0)
        {
            var colors = GradientColors.Select(color => color.ToSKColor()).ToArray();
            dialPaint.Shader = SKShader.CreateSweepGradient(_dialCenter, colors, SKShaderTileMode.Decal, 0.0f, MaxSweepAngle)
                .WithLocalMatrix(SKMatrix.CreateRotationDegrees(StartAngle, _dialCenter.X, _dialCenter.Y));
        }
        else
        {
            dialPaint.Color = DialColor.ToSKColor();
        }

        _canvas.DrawPath(dialPath, dialPaint);
       
    }

    private void DrawBase()
    {
        using var basePath = new SKPath();
        basePath.AddArc(_dialRect, BaseStartAngle, BaseSweepAngle);

        dashArray[0] = 1 * DialWidth;
        dashArray[1] = 1.5f * DialWidth;
        dashArray[2] = 1 * DialWidth;
        dashArray[3] = 1.5f * DialWidth;
        dashArray[3] = 1 * DialWidth;
        dashArray[3] = 1.5f * DialWidth;
        Enum.TryParse(StrokeCap, out SKStrokeCap cap);

     using SKPaint basePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = BaseColor.ToSKColor(),
            StrokeWidth = DialWidth,
            StrokeCap = cap,
           // PathEffect = StringToDialEffect(DialStyle),
            // PathEffect = SKPathEffect.CreateDash(dashArray, 10),
            IsAntialias = true

        };

        switch (DialStyle)
            {

            case "1DMorph":
                basePaint.PathEffect = morphPathEffect;
                break;
            case "Dash":
                basePaint.PathEffect = SKPathEffect.CreateDash(dashArray, DashPhase);
                break;
            case "none":
                basePaint.PathEffect = defaultPaintStyle;
                break;
        }


            _canvas.DrawPath(basePath, basePaint) ; 
        
    }


    protected override void OnTouch(SKTouchEventArgs e)
    {
        base.OnTouch(e);

        _hasTouch = true;

        _touchPoint = e.Location;

        InvalidateSurface();

        e.Handled = true;
    }

    private static void OnBindablePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((RadialDial)bindable).InvalidateSurface();
    }

    private static void OnTouchInputEnabledPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((RadialDial)bindable).EnableTouchEvents = (bool)newValue;
        ((RadialDial)bindable).InvalidateSurface();
    }

    #endregion
}