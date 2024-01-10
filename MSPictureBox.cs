using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace milano88.UI.Controls
{
    class MSPictureBox : Control
    {
        private BufferedGraphics bufGraphics;

        public MSPictureBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            Size = new Size(150, 150);
            BackColor = Color.Transparent;
            UpdateGraphicsBuffer();
        }

        private void UpdateGraphicsBuffer()
        {
            if (Width > 0 && Height > 0)
            {
                BufferedGraphicsContext context = BufferedGraphicsManager.Current;
                context.MaximumBuffer = new Size(Width + 1, Height + 1);
                bufGraphics = context.Allocate(CreateGraphics(), ClientRectangle);
            }
        }

        private Image image;
        [Category("Custom Properties")]
        [DefaultValue(null)]
        public Image Image
        {
            get => image;
            set { image = value; Invalidate(); }
        }

        private int borderSize;
        [Category("Custom Properties")]
        [DefaultValue(0)]
        public int BorderSize
        {
            get { return borderSize; }
            set
            {
                borderSize = value;
                if (borderRadius > (Height / 2) - borderSize)
                    borderRadius = (Height / 2) - borderSize;
                Invalidate();
            }
        }

        private int borderRadius;
        [Category("Custom Properties")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(0)]
        public int BorderRadius
        {
            get => borderRadius;
            set
            {
                if (borderSize == 0)
                    borderRadius = (value < 0) ? 0 : (value > Height / 2) ? Height / 2 : value;
                else borderRadius = (value < 0) ? 0 : (value > Height / 2) ? (Height / 2) - borderSize : value;

                Invalidate();
            }
        }

        private Color borderColor = Color.DodgerBlue;
        [Category("Custom Properties")]
        [DefaultValue(typeof(Color), "DodgerBlue")]
        public Color BorderColor
        {
            get => borderColor;
            set { borderColor = value; Invalidate(); }
        }

        private Color borderColor2 = Color.LightSteelBlue;
        [Category("Custom Properties")]
        [DefaultValue(typeof(Color), "LightSteelBlue")]
        public Color BorderColor2
        {
            get => borderColor2;
            set { borderColor2 = value; Invalidate(); }
        }

        private float borderGradientAngle = 360F;
        [Category("Custom Properties")]
        [DefaultValue(360F)]
        public float BorderGradientAngle
        {
            get => borderGradientAngle;
            set { borderGradientAngle = value; Invalidate(); }
        }

        private bool imageFilter = false;
        [Category("Custom Properties")]
        [DefaultValue(typeof(bool), "False")]
        public bool ImageFilter
        {
            get => imageFilter;
            set { imageFilter = value; Invalidate(); }
        }

        private Color filterColor = Color.DodgerBlue;
        [Category("Custom Properties")]
        [DefaultValue(typeof(Color), "DodgerBlue")]
        public Color FiterColor1
        {
            get => filterColor;
            set { filterColor = value; Invalidate(); }
        }

        private Color filterColor2 = Color.White;
        [Category("Custom Properties")]
        [DefaultValue(typeof(Color), "White")]
        public Color FiterColor2
        {
            get => filterColor2;
            set { filterColor2 = value; Invalidate(); }
        }

        private int filterAlpha = 100;
        [Category("Custom Properties")]
        [DefaultValue(100)]
        public int FilterTransparency
        {
            get => filterAlpha;
            set { filterAlpha = value; Invalidate(); }
        }

        private float filterAngle = 180F;
        [Category("Custom Properties")]
        [DefaultValue(180F)]
        public float FilterAngle
        {
            get => filterAngle;
            set { filterAngle = value; Invalidate(); }
        }

        [Browsable(false)]
        public override Image BackgroundImage { get => base.BackgroundImage; set { } }
        [Browsable(false)]
        public override ImageLayout BackgroundImageLayout { get => base.BackgroundImageLayout; set { } }
        [Browsable(false)]
        public override Color ForeColor { get => base.ForeColor; set { } }
        [Browsable(false)]
        public override string Text { get => base.Text; set { } }
        [Browsable(false)]
        public override Font Font { get => base.Font; set { } }
        [DefaultValue(typeof(Color), "Transparent")]
        public override Color BackColor { get => base.BackColor; set => base.BackColor = value; }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (borderRadius > (Height / 2) - borderSize)
                borderRadius = (Height / 2) - borderSize;

            UpdateGraphicsBuffer();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (image != null)
            {
                bufGraphics.Graphics.Clear(Parent.BackColor);

                if (borderRadius > 0)
                {
                    using (GraphicsPath pathBorder = RoundedRectCreate(ClientRectangle, borderRadius, borderSize))
                    using (LinearGradientBrush borderBrush = new LinearGradientBrush(ClientRectangle, borderColor, borderColor2, borderGradientAngle))
                    using (Bitmap bitmap = new Bitmap(image, Width, Height))
                    using (TextureBrush textureBrush = new TextureBrush(bitmap))
                    using (Pen penBorder = new Pen(borderBrush, borderSize))
                    {
                        bufGraphics.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        bufGraphics.Graphics.FillPath(textureBrush, pathBorder);

                        if (imageFilter)
                        {
                            using (LinearGradientBrush brushFilter = new LinearGradientBrush(ClientRectangle, Color.FromArgb(filterAlpha, filterColor), Color.FromArgb(filterAlpha, filterColor2), filterAngle))
                                bufGraphics.Graphics.FillPath(brushFilter, pathBorder);
                        }

                        if (borderSize > 0)
                            bufGraphics.Graphics.DrawPath(penBorder, pathBorder);
                    }
                }
                else
                {
                    bufGraphics.Graphics.DrawImage(image, ClientRectangle);

                    if (imageFilter)
                    {
                        using (LinearGradientBrush brushFilter = new LinearGradientBrush(ClientRectangle, Color.FromArgb(filterAlpha, filterColor), Color.FromArgb(filterAlpha, filterColor2), filterAngle))
                            bufGraphics.Graphics.FillRectangle(brushFilter, ClientRectangle);
                    }

                    if (borderSize > 0)
                    {
                        bufGraphics.Graphics.SmoothingMode = SmoothingMode.None;
                        using (LinearGradientBrush borderBrush = new LinearGradientBrush(ClientRectangle, borderColor, borderColor2, borderGradientAngle))
                        using (Pen pen = new Pen(borderBrush, borderSize) { Alignment = PenAlignment.Inset })
                            bufGraphics.Graphics.DrawRectangle(pen, ClientRectangle);
                    }

                }
            }
            else
            {
                using (SolidBrush brush = new SolidBrush(Color.White))
                    bufGraphics.Graphics.FillRectangle(brush, ClientRectangle);
            }

            bufGraphics.Render(e.Graphics);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (Parent != null && BackColor == Color.Transparent)
            {
                Rectangle rect = new Rectangle(Left, Top, Width, Height);
                bufGraphics.Graphics.TranslateTransform(-rect.X, -rect.Y);
                try
                {
                    using (PaintEventArgs pea = new PaintEventArgs(bufGraphics.Graphics, rect))
                    {
                        pea.Graphics.SetClip(rect);
                        InvokePaintBackground(Parent, pea);
                        InvokePaint(Parent, pea);
                    }
                }
                finally
                {
                    bufGraphics.Graphics.TranslateTransform(rect.X, rect.Y);
                }
            }
            else
            {
                using (SolidBrush backColor = new SolidBrush(this.BackColor))
                    bufGraphics.Graphics.FillRectangle(backColor, ClientRectangle);
            }
        }

        private GraphicsPath RoundedRectCreate(Rectangle rect, float radius, float stroke = 0F)
        {
            rect = new Rectangle(0, 0, rect.Width - 1, rect.Height - 1);
            float diameter = radius * 2F;
            GraphicsPath path = new GraphicsPath();
            if (stroke >= 2F)
            {
                path.StartFigure();
                path.AddArc(rect.X + (stroke / 2), rect.Y + (stroke / 2), diameter, diameter, 180, 90);
                path.AddArc(rect.Right - diameter - (stroke / 2), rect.Y + (stroke / 2), diameter, diameter, 270, 90);
                path.AddArc(rect.Right - diameter - (stroke / 2), rect.Bottom - diameter - (stroke / 2), diameter, diameter, 0, 90);
                path.AddArc(rect.X + (stroke / 2), rect.Bottom - diameter - (stroke / 2), diameter, diameter, 90, 90);
                path.CloseFigure();
            }
            else
            {
                path.StartFigure();
                path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
                path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
                path.CloseFigure();
            }
            return path;
        }
    }
}
