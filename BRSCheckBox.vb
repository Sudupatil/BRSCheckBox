Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

Partial Public Class BRSCheckBox
    Inherits UserControl

#Region "Enums"
    ' 1. First, update your ToggleStyle enum to include MetroFramework
    Public Enum ToggleStyle
        Standerd
        Square
        Modern
        Slim
        'MaterialToggle
        Rectangle
        Metro
    End Enum
#End Region

#Region "Private Fields"
    Private _checked As Boolean = False
    Private _style As ToggleStyle = ToggleStyle.Modern
    Private _uncheckedBackColor As Color = Color.FromArgb(200, 200, 200)
    Private _checkedBackColor As Color = Color.FromArgb(0, 122, 255)
    Private _sliderColor As Color = Color.White
    Private _showStatus As Boolean = True
    Private _uncheckedText As String = "OFF"
    Private _checkedText As String = "ON"
    Private _uncheckedTextColor As Color = Color.Gray
    Private _checkedTextColor As Color = Color.Black
    Private _uncheckedFont As Font
    Private _checkedFont As Font
    Private _animating As Boolean = False
    Private _animationTimer As Timer
    Private _animationProgress As Single = 0.0F
    Private _isHovered As Boolean = False
    Private components As System.ComponentModel.IContainer
    Private _borderColor As Color = Color.FromArgb(180, 180, 180)
    Private _focusColor As Color = Color.FromArgb(0, 122, 255)
#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()

        SetStyle(ControlStyles.AllPaintingInWmPaint Or
                ControlStyles.UserPaint Or
                ControlStyles.ResizeRedraw Or
                ControlStyles.DoubleBuffer Or
                ControlStyles.SupportsTransparentBackColor, True)

        Size = New Size(120, 30)
        _uncheckedFont = New Font("Segoe UI", 8.25F, FontStyle.Regular)
        _checkedFont = New Font("Segoe UI", 8.25F, FontStyle.Regular)

        ' Initialize animation timer
        _animationTimer = New Timer()
        _animationTimer.Interval = 15
        AddHandler _animationTimer.Tick, AddressOf AnimationTimer_Tick

        BackColor = Color.Transparent
    End Sub
#End Region

#Region "Properties"
    <Category("Appearance")>
    <Description("Gets or sets whether the toggle is checked")>
    Public Property Checked As Boolean
        Get
            Return _checked
        End Get
        Set(value As Boolean)
            If _checked <> value Then
                _checked = value
                StartAnimation()
                OnCheckedChanged(EventArgs.Empty)
            End If
        End Set
    End Property

    <Category("Appearance")>
    <Description("Gets or sets the toggle style")>
    Public Property Style As ToggleStyle
        Get
            Return _style
        End Get
        Set(value As ToggleStyle)
            _style = value
            Invalidate()
        End Set
    End Property

    <Category("Appearance")>
    <Description("Gets or sets the background color when unchecked")>
    Public Property UncheckedBackColor As Color
        Get
            Return _uncheckedBackColor
        End Get
        Set(value As Color)
            _uncheckedBackColor = value
            Invalidate()
        End Set
    End Property

    <Category("Appearance")>
    <Description("Gets or sets the background color when checked")>
    Public Property CheckedBackColor As Color
        Get
            Return _checkedBackColor
        End Get
        Set(value As Color)
            _checkedBackColor = value
            Invalidate()
        End Set
    End Property

    <Category("Appearance")>
    <Description("Gets or sets the slider color")>
    Public Property SliderColor As Color
        Get
            Return _sliderColor
        End Get
        Set(value As Color)
            _sliderColor = value
            Invalidate()
        End Set
    End Property

    <Category("Appearance")>
    <Description("Gets or sets the border color")>
    Public Property BorderColor As Color
        Get
            Return _borderColor
        End Get
        Set(value As Color)
            _borderColor = value
            Invalidate()
        End Set
    End Property

    <Category("Appearance")>
    <Description("Gets or sets whether to show status text")>
    Public Property ShowStatus As Boolean
        Get
            Return _showStatus
        End Get
        Set(value As Boolean)
            _showStatus = value
            Invalidate()
        End Set
    End Property

    <Category("Appearance")>
    <Description("Gets or sets the text displayed when unchecked")>
    Public Property UncheckedText As String
        Get
            Return _uncheckedText
        End Get
        Set(value As String)
            _uncheckedText = value
            Invalidate()
        End Set
    End Property

    <Category("Appearance")>
    <Description("Gets or sets the text displayed when checked")>
    Public Property CheckedText As String
        Get
            Return _checkedText
        End Get
        Set(value As String)
            _checkedText = value
            Invalidate()
        End Set
    End Property

    <Category("Appearance")>
    <Description("Gets or sets the text color when unchecked")>
    Public Property UncheckedTextColor As Color
        Get
            Return _uncheckedTextColor
        End Get
        Set(value As Color)
            _uncheckedTextColor = value
            Invalidate()
        End Set
    End Property

    <Category("Appearance")>
    <Description("Gets or sets the text color when checked")>
    Public Property CheckedTextColor As Color
        Get
            Return _checkedTextColor
        End Get
        Set(value As Color)
            _checkedTextColor = value
            Invalidate()
        End Set
    End Property

    <Category("Appearance")>
    <Description("Gets or sets the font when unchecked")>
    Public Property UncheckedFont As Font
        Get
            Return _uncheckedFont
        End Get
        Set(value As Font)
            _uncheckedFont = value
            Invalidate()
        End Set
    End Property

    <Category("Appearance")>
    <Description("Gets or sets the font when checked")>
    Public Property CheckedFont As Font
        Get
            Return _checkedFont
        End Get
        Set(value As Font)
            _checkedFont = value
            Invalidate()
        End Set
    End Property
#End Region

#Region "Events"
    <Category("Action")>
    <Description("Occurs when the Checked property changes")>
    Public Event CheckedChanged As EventHandler

    Protected Overridable Sub OnCheckedChanged(e As EventArgs)
        RaiseEvent CheckedChanged(Me, e)
    End Sub
#End Region

#Region "Animation"
    Private Sub StartAnimation()
        _animationProgress = 0.0F
        _animating = True
        _animationTimer.Start()
    End Sub

    Private Sub AnimationTimer_Tick(sender As Object, e As EventArgs)
        _animationProgress += 0.08F
        If _animationProgress >= 1.0F Then
            _animationProgress = 1.0F
            _animating = False
            _animationTimer.Stop()
        End If
        Invalidate()
    End Sub
#End Region

#Region "Paint Methods"
    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim g As Graphics = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit

        Dim toggleHeight As Integer = Me.Height - 10 ' Leave some padding
        Dim toggleWidth As Integer = toggleHeight * 2 ' Maintain a 2:1 aspect ratio

        ' 2. Add the case in your OnPaint method's Select Case statement
        Select Case _style
            Case ToggleStyle.Standerd
                DrawDefaultCheckbox(g, toggleHeight, toggleWidth)
            Case ToggleStyle.Modern
                DrawModernToggle(g, toggleHeight, toggleWidth)
            Case ToggleStyle.Slim
                DrawSlimToggle(g, toggleHeight, toggleWidth)
            Case ToggleStyle.Square
                DrawSquareToggle(g, toggleHeight, toggleWidth)
            'Case ToggleStyle.MaterialToggle
            '    DrawMaterialToggle(g, toggleHeight, toggleWidth)
            Case ToggleStyle.Rectangle
                DrawRectangleToggle(g, toggleHeight, toggleWidth)
            Case ToggleStyle.Metro
                DrawMetroToggle(g, toggleHeight, toggleWidth)
        End Select

        ' Draw status text
        If _showStatus Then
            DrawStatusText(g, toggleWidth + 10)
        End If
    End Sub

    Private Sub DrawDefaultCheckbox(g As Graphics, toggleHeight As Integer, toggleWidth As Integer)
        Dim checkboxSize As Integer = toggleHeight
        Dim checkboxRect As New Rectangle(5, (Me.Height - checkboxSize) \ 2, checkboxSize, checkboxSize)

        ' Draw checkbox background
        Dim backColor As Color = If(_checked, _checkedBackColor, Color.White)
        Using brush As New SolidBrush(backColor)
            g.FillRoundedRectangle(brush, checkboxRect, 3)
        End Using

        ' Draw border
        Dim borderColor As Color = If(_checked, _checkedBackColor, _borderColor)
        Using pen As New Pen(borderColor, 1)
            g.DrawRoundedRectangle(pen, checkboxRect, 3)
        End Using

        ' Draw checkmark if checked
        If _checked Then
            Using pen As New Pen(Color.White, 1)
                pen.StartCap = LineCap.Round
                pen.EndCap = LineCap.Round
                Dim checkPoints() As Point = {
                New Point(checkboxRect.X + 4, checkboxRect.Y + checkboxRect.Height \ 2),
                New Point(checkboxRect.X + checkboxRect.Width \ 2 - 1, checkboxRect.Y + checkboxRect.Height - 6),
                New Point(checkboxRect.X + checkboxRect.Width - 4, checkboxRect.Y + 4)
            }
                g.DrawLines(pen, checkPoints)
            End Using
        End If
    End Sub

    Private Sub DrawModernToggle(g As Graphics, toggleHeight As Integer, toggleWidth As Integer)
        Dim toggleRect As New Rectangle(5, (Me.Height - toggleHeight) \ 2, toggleWidth, toggleHeight)

        ' Calculate slider position with smooth animation
        Dim sliderSize As Integer = toggleHeight - 4
        Dim sliderPadding As Integer = 2
        Dim sliderX As Single

        If _animating Then
            Dim startX As Single = If(Not _checked, toggleRect.X + sliderPadding, toggleRect.Right - sliderSize - sliderPadding)
            Dim endX As Single = If(_checked, toggleRect.Right - sliderSize - sliderPadding, toggleRect.X + sliderPadding)
            ' Ease-out animation
            Dim easedProgress As Single = 1 - CSng(Math.Pow(1 - _animationProgress, 3))
            sliderX = startX + (endX - startX) * easedProgress
        Else
            sliderX = If(_checked, toggleRect.Right - sliderSize - sliderPadding, toggleRect.X + sliderPadding)
        End If

        ' Draw toggle background with gradient
        Dim backColor As Color = If(_checked, _checkedBackColor, _uncheckedBackColor)
        Using brush As New LinearGradientBrush(toggleRect,
                                           ColorHelper.Lighten(backColor, 0.1F),
                                           ColorHelper.Darken(backColor, 0.05F),
                                           LinearGradientMode.Vertical)
            g.FillRoundedRectangle(brush, toggleRect, toggleHeight \ 2)
        End Using

        ' Draw subtle inner shadow
        If Not _checked Then
            Using shadowBrush As New SolidBrush(Color.FromArgb(20, 0, 0, 0))
                Dim innerShadowRect As New Rectangle(toggleRect.X + 1, toggleRect.Y + 1, toggleRect.Width - 2, toggleRect.Height - 2)
                g.FillRoundedRectangle(shadowBrush, innerShadowRect, (toggleHeight - 2) \ 2)
            End Using
        End If

        ' Draw slider with shadow
        Dim sliderRect As New Rectangle(CInt(sliderX), toggleRect.Y + sliderPadding, sliderSize, sliderSize)

        ' Drop shadow
        Dim dropShadowRect As New Rectangle(sliderRect.X + 1, sliderRect.Y + 2, sliderRect.Width, sliderRect.Height)
        Using shadowBrush As New SolidBrush(Color.FromArgb(30, 0, 0, 0))
            g.FillEllipse(shadowBrush, dropShadowRect)
        End Using

        ' Main slider
        Using brush As New LinearGradientBrush(sliderRect, Color.White, Color.FromArgb(245, 245, 245), LinearGradientMode.Vertical)
            g.FillEllipse(brush, sliderRect)
        End Using

        ' Slider border
        Using pen As New Pen(Color.FromArgb(200, 200, 200), 1)
            g.DrawEllipse(pen, sliderRect)
        End Using
    End Sub

    Private Sub DrawSlimToggle(g As Graphics, toggleHeight As Integer, toggleWidth As Integer)

        ' Make the slider bigger
        Dim sliderSize As Integer = toggleHeight

        ' Make the track smaller in height
        Dim trackHeight As Integer = Me.Height / 2.5
        Dim toggleRect As New Rectangle(5, (Me.Height - trackHeight) \ 2, toggleWidth, trackHeight)
        ' Calculate slider position
        Dim sliderPadding As Integer = 1
        Dim sliderX As Single

        If _animating Then
            Dim startX As Single = If(Not _checked, toggleRect.X, toggleRect.Right - sliderSize)
            Dim endX As Single = If(_checked, toggleRect.Right - sliderSize, toggleRect.X)
            sliderX = startX + (endX - startX) * _animationProgress
        Else
            sliderX = If(_checked, toggleRect.Right - sliderSize, toggleRect.X)
        End If

        ' Draw track
        Dim trackColor As Color = If(_checked, ColorHelper.SetAlpha(_checkedBackColor, 128), _uncheckedBackColor)
        Using brush As New SolidBrush(trackColor)
            g.FillRoundedRectangle(brush, toggleRect, trackHeight \ 2)
        End Using

        ' Draw slider with material design ripple effect
        'Dim sliderRect As New Rectangle(CInt(sliderX), (Me.Height - sliderSize) \ 2, sliderSize, sliderSize)
        Dim sliderRect As New Rectangle(CInt(sliderX), (Me.Height - (sliderSize - 2)) \ 2, sliderSize - 2, sliderSize - 2)

        ' Ripple effect when hovered
        If _isHovered Then
            Dim rippleSize As Integer = sliderSize + 8
            Dim rippleRect As New Rectangle(sliderRect.X - 4, sliderRect.Y - 4, rippleSize, rippleSize)
            Using rippleBrush As New SolidBrush(Color.FromArgb(30, _focusColor.R, _focusColor.G, _focusColor.B))
                g.FillEllipse(rippleBrush, rippleRect)
            End Using
        End If

        ' Main slider
        Dim sliderColor As Color = If(_checked, _checkedBackColor, _sliderColor)
        Using brush As New SolidBrush(sliderColor)
            g.FillEllipse(brush, sliderRect)
        End Using
    End Sub

    Private Sub DrawSquareToggle(g As Graphics, toggleHeight As Integer, toggleWidth As Integer)
        Dim toggleRect As New Rectangle(5, (Me.Height - toggleHeight) \ 2, toggleHeight, toggleHeight)

        ' Draw background rectangle
        Dim backColor As Color = If(_checked, _checkedBackColor, _uncheckedBackColor)
        Using brush As New SolidBrush(backColor)
            g.FillRectangle(brush, toggleRect)
        End Using

        ' Draw border
        Using pen As New Pen(_borderColor, 1)
            g.DrawRectangle(pen, toggleRect)
        End Using

        ' Draw inner rectangle if checked with animation
        If _checked OrElse _animating Then
            Dim innerSize As Integer = CInt(toggleHeight * 0.4 * If(_animating, _animationProgress, 1.0F))
            If innerSize > 0 Then
                Dim innerRect As New Rectangle(
                toggleRect.X + (toggleRect.Width - innerSize) \ 2,
                toggleRect.Y + (toggleRect.Height - innerSize) \ 2,
                innerSize, innerSize)
                Using brush As New SolidBrush(Color.White)
                    g.FillRectangle(brush, innerRect)
                End Using
            End If
        End If
    End Sub

    Private Sub DrawRectangleToggle(g As Graphics, toggleHeight As Integer, toggleWidth As Integer)
        ' Metro-style toggle based on Windows 8/10 design principles
        ' Characteristics: Flat design, no gradients, sharp corners, high contrast

        Dim toggleRect As New Rectangle(5, (Me.Height - toggleHeight) \ 2, toggleWidth, toggleHeight)

        ' Metro colors - use flat, high-contrast colors
        Dim metroCheckedColor As Color = Color.FromArgb(0, 120, 215) ' Windows 10 blue
        Dim metroUncheckedColor As Color = Color.FromArgb(76, 76, 76) ' Dark gray
        Dim metroSliderColor As Color = Color.White

        ' Override with user colors if set
        Dim backColor As Color = If(_checked, _checkedBackColor, _uncheckedBackColor)
        If _checked AndAlso _checkedBackColor = Color.FromArgb(0, 122, 255) Then
            backColor = metroCheckedColor ' Use Metro blue if default
        ElseIf Not _checked AndAlso _uncheckedBackColor = Color.FromArgb(200, 200, 200) Then
            backColor = metroUncheckedColor ' Use Metro gray if default
        End If

        ' Draw flat background with sharp corners (no rounded rectangles)
        Using brush As New SolidBrush(backColor)
            g.FillRectangle(brush, toggleRect)
        End Using

        ' Calculate slider position and size
        Dim sliderWidth As Integer = CInt(toggleWidth * 0.45) ' Slightly smaller than half
        Dim sliderHeight As Integer = toggleHeight - 6 ' Leave small margin
        Dim sliderY As Integer = toggleRect.Y + 3
        Dim sliderX As Single

        If _animating Then
            Dim startX As Single = If(Not _checked, toggleRect.X + 3, toggleRect.Right - sliderWidth - 3)
            Dim endX As Single = If(_checked, toggleRect.Right - sliderWidth - 3, toggleRect.X + 3)
            ' Use linear animation (no easing) - Metro style is direct
            sliderX = startX + (endX - startX) * _animationProgress
        Else
            sliderX = If(_checked, toggleRect.Right - sliderWidth - 3, toggleRect.X + 3)
        End If

        ' Draw the slider with flat design
        Dim sliderRect As New Rectangle(CInt(sliderX), sliderY, sliderWidth, sliderHeight)

        ' Use white slider for checked state, light gray for unchecked
        Dim currentSliderColor As Color = If(_checked, metroSliderColor, Color.FromArgb(220, 220, 220))
        If _sliderColor <> Color.White Then
            currentSliderColor = _sliderColor ' Use custom color if set
        End If

        Using brush As New SolidBrush(currentSliderColor)
            g.FillRectangle(brush, sliderRect)
        End Using

        ' Optional: Add subtle border for definition (Metro style sometimes uses thin borders)
        If Not _checked Then
            Using pen As New Pen(Color.FromArgb(160, 160, 160), 1)
                g.DrawRectangle(pen, toggleRect)
            End Using
        End If

        ' Add focus indicator when hovered (Metro style)
        If _isHovered Then
            Using pen As New Pen(Color.FromArgb(100, If(_checked, metroCheckedColor, Color.Black)), 2)
                Dim focusRect As New Rectangle(toggleRect.X - 1, toggleRect.Y - 1,
                                         toggleRect.Width + 2, toggleRect.Height + 2)
                g.DrawRectangle(pen, focusRect)
            End Using
        End If
    End Sub

    ' 4. Also implement the MaterialToggle method since it was commented out
    Private Sub DrawMaterialToggle(g As Graphics, toggleHeight As Integer, toggleWidth As Integer)
        Dim toggleRect As New Rectangle(5, (Me.Height - CInt(toggleHeight * 0.6)) \ 2, toggleWidth, CInt(toggleHeight * 0.6))

        ' Calculate slider position
        Dim sliderSize As Integer = toggleHeight + 2 ' Slightly larger than track
        Dim sliderPadding As Integer = 0
        Dim sliderX As Single

        If _animating Then
            Dim startX As Single = If(Not _checked, toggleRect.X - 1, toggleRect.Right - sliderSize + 1)
            Dim endX As Single = If(_checked, toggleRect.Right - sliderSize + 1, toggleRect.X - 1)
            ' Material Design uses ease-out animation
            Dim easedProgress As Single = 1 - CSng(Math.Pow(1 - _animationProgress, 2))
            sliderX = startX + (endX - startX) * easedProgress
        Else
            sliderX = If(_checked, toggleRect.Right - sliderSize + 1, toggleRect.X - 1)
        End If

        ' Draw track with Material Design colors
        Dim trackColor As Color = If(_checked,
                               ColorHelper.SetAlpha(_checkedBackColor, 128),
                               Color.FromArgb(189, 189, 189))
        Using brush As New SolidBrush(trackColor)
            g.FillRoundedRectangle(brush, toggleRect, toggleRect.Height \ 2)
        End Using

        ' Draw slider shadow first (Material Design elevation)
        Dim sliderRect As New Rectangle(CInt(sliderX), (Me.Height - sliderSize) \ 2, sliderSize, sliderSize)
        Dim shadowRect As New Rectangle(sliderRect.X + 1, sliderRect.Y + 2, sliderRect.Width, sliderRect.Height)
        Using shadowBrush As New SolidBrush(Color.FromArgb(40, 0, 0, 0))
            g.FillEllipse(shadowBrush, shadowRect)
        End Using

        ' Ripple effect when hovered (Material Design interaction)
        If _isHovered Then
            Dim rippleSize As Integer = sliderSize + 12
            Dim rippleRect As New Rectangle(sliderRect.X - 6, sliderRect.Y - 6, rippleSize, rippleSize)
            Dim rippleColor As Color = If(_checked,
                                    ColorHelper.SetAlpha(_checkedBackColor, 30),
                                    Color.FromArgb(30, 0, 0, 0))
            Using rippleBrush As New SolidBrush(rippleColor)
                g.FillEllipse(rippleBrush, rippleRect)
            End Using
        End If

        ' Main slider with Material Design colors
        Dim sliderColor As Color = If(_checked, _checkedBackColor, Color.FromArgb(250, 250, 250))
        Using brush As New SolidBrush(sliderColor)
            g.FillEllipse(brush, sliderRect)
        End Using

        ' Slider border for unchecked state
        If Not _checked Then
            Using pen As New Pen(Color.FromArgb(189, 189, 189), 1)
                g.DrawEllipse(pen, sliderRect)
            End Using
        End If
    End Sub

    Private Sub DrawMetroToggle(g As Graphics, toggleHeight As Integer, toggleWidth As Integer)
        ' Authentic MetroFramework toggle design:
        ' - Horizontal track (rectangle)
        ' - Vertical slider (taller rectangle) that moves horizontally

        ' Track dimensions - horizontal rectangle, thinner than control height
        Dim trackHeight As Integer = CInt(toggleHeight * 0.4) ' About 40% of control height
        Dim trackWidth As Integer = toggleWidth
        Dim trackRect As New Rectangle(5, (Me.Height - trackHeight) \ 2, trackWidth, trackHeight)

        ' Metro colors - flat design with high contrast
        Dim metroCheckedColor As Color = Color.FromArgb(0, 120, 215) ' Windows 10 blue
        Dim metroUncheckedColor As Color = Color.FromArgb(195, 195, 195) ' Light gray track
        Dim metroSliderColor As Color = Color.White

        ' Use custom colors if different from defaults
        Dim trackColor As Color = If(_checked, _checkedBackColor, _uncheckedBackColor)
        If _checked AndAlso _checkedBackColor = Color.FromArgb(0, 122, 255) Then
            trackColor = metroCheckedColor
        ElseIf Not _checked AndAlso _uncheckedBackColor = Color.FromArgb(200, 200, 200) Then
            trackColor = metroUncheckedColor
        End If

        ' Draw horizontal track
        Using brush As New SolidBrush(trackColor)
            g.FillRectangle(brush, trackRect)
        End Using

        ' Draw track border
        Using pen As New Pen(Color.FromArgb(160, 160, 160), 1)
            g.DrawRectangle(pen, trackRect)
        End Using

        ' Slider dimensions - vertical rectangle, taller than track
        Dim sliderWidth As Integer = CInt(trackWidth * 0.3) ' About 30% of track width
        Dim sliderHeight As Integer = CInt(toggleHeight * 0.8) ' 80% of control height (taller than track)
        Dim sliderY As Integer = (Me.Height - sliderHeight) \ 2
        Dim sliderX As Single

        ' Calculate slider position with animation
        If _animating Then
            Dim startX As Single = If(Not _checked, trackRect.X, trackRect.Right - sliderWidth)
            Dim endX As Single = If(_checked, trackRect.Right - sliderWidth, trackRect.X)
            ' Linear animation for Metro style
            sliderX = startX + (endX - startX) * _animationProgress
        Else
            sliderX = If(_checked, trackRect.Right - sliderWidth, trackRect.X)
        End If

        ' Draw vertical slider rectangle
        Dim sliderRect As New Rectangle(CInt(sliderX), sliderY, sliderWidth, sliderHeight)

        ' Slider color - white for authentic Metro look
        Dim currentSliderColor As Color = metroSliderColor
        If _sliderColor <> Color.White Then
            currentSliderColor = _sliderColor
        End If

        Using brush As New SolidBrush(currentSliderColor)
            g.FillRectangle(brush, sliderRect)
        End Using

        ' Draw slider border
        Using pen As New Pen(Color.FromArgb(140, 140, 140), 1)
            g.DrawRectangle(pen, sliderRect)
        End Using

        If _isHovered Then
            ' Create a slightly larger rectangle around the slider for hover effect
            Using pen As New Pen(Color.FromArgb(80, If(_checked, metroCheckedColor, Color.Black)), 2)
                Dim hoverRect As New Rectangle(sliderRect.X - 1, sliderRect.Y - 1,
                                             sliderRect.Width + 2, sliderRect.Height + 2)
                g.DrawRectangle(pen, hoverRect)
            End Using
        End If
    End Sub

    Private Sub DrawStatusText(g As Graphics, x As Integer)
        Dim text As String = If(_checked, _checkedText, _uncheckedText)
        Dim textColor As Color = If(_checked, _checkedTextColor, _uncheckedTextColor)
        Dim font As Font = If(_checked, _checkedFont, _uncheckedFont)

        Dim textSize As SizeF = g.MeasureString(text, font)
        Dim textY As Single = (Me.Height - textSize.Height) / 2
        'Dim x As Single
        Select Case _style
            Case ToggleStyle.Standerd, ToggleStyle.Square
                x = 36 ' or some other value that works for ModernToggle and SlimToggle
                'Case Else
                '    x = 24 ' or some other value that works for other styles
        End Select

        Using brush As New SolidBrush(textColor)
            g.DrawString(text, font, brush, x, textY)
        End Using
    End Sub
#End Region

#Region "Mouse Events"
    Protected Overrides Sub OnMouseClick(e As MouseEventArgs)
        MyBase.OnMouseClick(e)
        If e.Button = MouseButtons.Left Then
            Checked = Not Checked
        End If
    End Sub

    Protected Overrides Sub OnMouseEnter(e As EventArgs)
        MyBase.OnMouseEnter(e)
        _isHovered = True
        Cursor = Cursors.Hand
        Invalidate()
    End Sub

    Protected Overrides Sub OnMouseLeave(e As EventArgs)
        MyBase.OnMouseLeave(e)
        _isHovered = False
        Cursor = Cursors.Default
        Invalidate()
    End Sub
#End Region

#Region "Dispose"
    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            _animationTimer?.Dispose()
            _uncheckedFont?.Dispose()
            _checkedFont?.Dispose()
            If components IsNot Nothing Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub
#End Region

#Region "Designer"
    Private Sub InitializeComponent()
        components = New System.ComponentModel.Container()
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    End Sub
#End Region
End Class

#Region "Helper Classes"
Public Module GraphicsExtensions
    <System.Runtime.CompilerServices.Extension>
    Public Sub FillRoundedRectangle(g As Graphics, brush As Brush, rect As Rectangle, radius As Integer)
        Using path As New GraphicsPath()
            AddRoundedRectangle(path, rect, radius)
            g.FillPath(brush, path)
        End Using
    End Sub

    <System.Runtime.CompilerServices.Extension>
    Public Sub DrawRoundedRectangle(g As Graphics, pen As Pen, rect As Rectangle, radius As Integer)
        Using path As New GraphicsPath()
            AddRoundedRectangle(path, rect, radius)
            g.DrawPath(pen, path)
        End Using
    End Sub

    Private Sub AddRoundedRectangle(path As GraphicsPath, rect As Rectangle, radius As Integer)
        Dim diameter As Integer = radius * 2
        Dim arc As New Rectangle(rect.Location, New Size(diameter, diameter))

        ' Top left arc
        path.AddArc(arc, 180, 90)

        ' Top right arc
        arc.X = rect.Right - diameter
        path.AddArc(arc, 270, 90)

        ' Bottom right arc
        arc.Y = rect.Bottom - diameter
        path.AddArc(arc, 0, 90)

        ' Bottom left arc
        arc.X = rect.Left
        path.AddArc(arc, 90, 90)

        path.CloseFigure()
    End Sub
End Module

Public Module ColorHelper
    Public Function Lighten(color As Color, factor As Single) As Color
        Return Color.FromArgb(color.A,
                             Math.Min(255, CInt(color.R + (255 - color.R) * factor)),
                             Math.Min(255, CInt(color.G + (255 - color.G) * factor)),
                             Math.Min(255, CInt(color.B + (255 - color.B) * factor)))
    End Function

    Public Function Darken(color As Color, factor As Single) As Color
        Return Color.FromArgb(color.A,
                             Math.Max(0, CInt(color.R * (1 - factor))),
                             Math.Max(0, CInt(color.G * (1 - factor))),
                             Math.Max(0, CInt(color.B * (1 - factor))))
    End Function

    Public Function SetAlpha(color As Color, alpha As Integer) As Color
        Return Color.FromArgb(alpha, color.R, color.G, color.B)
    End Function
End Module
#End Region