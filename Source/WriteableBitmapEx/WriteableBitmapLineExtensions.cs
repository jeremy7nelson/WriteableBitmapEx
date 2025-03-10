﻿#region Header
//
//   Project:           WriteableBitmapEx - WriteableBitmap extensions
//   Description:       Collection of draw line extension and helper methods for the WriteableBitmap class.
//
//   Changed by:        $Author: unknown $
//   Changed on:        $Date: 2015-02-24 20:36:41 +0100 (Di, 24 Feb 2015) $
//   Changed in:        $Revision: 112951 $
//   Project:           $URL: https://writeablebitmapex.svn.codeplex.com/svn/trunk/Source/WriteableBitmapEx/WriteableBitmapTransformationExtensions.cs $
//   Id:                $Id: WriteableBitmapTransformationExtensions.cs 112951 2015-02-24 19:36:41Z unknown $
//
//
//   Copyright © 2009-2015 Rene Schulte and WriteableBitmapEx Contributors
//
//   This code is open source. Please read the License.txt for details. No worries, we won't sue you! ;)
//
#endregion

namespace System.Windows.Media.Imaging
{
    public static unsafe partial class WriteableBitmapExtensions
    {
        #region Normal line

        /// <summary>
        /// Draws a colored line by connecting two points using the Bresenham algorithm.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
        /// <param name="clipRect">The region in the image to restrict drawing to.</param>
        public static void DrawLineBresenham(this WriteableBitmap bmp, int x1, int y1, int x2, int y2, Color color, Rect? clipRect = null)
        {
            var col = ConvertColor(color);
            bmp.DrawLineBresenham(x1, y1, x2, y2, col, clipRect);
        }

        /// <summary>
        /// Draws a colored line by connecting two points using the Bresenham algorithm.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
        /// <param name="clipRect">The region in the image to restrict drawing to.</param>
        public static void DrawLineBresenham(this WriteableBitmap bmp, int x1, int y1, int x2, int y2, int color, Rect? clipRect = null)
        {
            using var context = bmp.GetBitmapContext();
            // Use refs for faster access (really important!) speeds up a lot!
            int w = context.Width;
            int h = context.Height;
            var pixels = context.Pixels;

            // Get clip coordinates
            int clipX1 = 0;
            int clipX2 = w;
            int clipY1 = 0;
            int clipY2 = h;
            if (clipRect.HasValue)
            {
                var c = clipRect.Value;
                clipX1 = (int)c.X;
                clipX2 = (int)(c.X + c.Width);
                clipY1 = (int)c.Y;
                clipY2 = (int)(c.Y + c.Height);
            }

            // Distance start and end point
            int dx = x2 - x1;
            int dy = y2 - y1;

            // Determine sign for direction x
            int incx = 0;
            if (dx < 0)
            {
                dx = -dx;
                incx = -1;
            }
            else if (dx > 0)
            {
                incx = 1;
            }

            // Determine sign for direction y
            int incy = 0;
            if (dy < 0)
            {
                dy = -dy;
                incy = -1;
            }
            else if (dy > 0)
            {
                incy = 1;
            }

            // Which gradient is larger
            int pdx, pdy, odx, ody, es, el;
            if (dx > dy)
            {
                pdx = incx;
                pdy = 0;
                odx = incx;
                ody = incy;
                es = dy;
                el = dx;
            }
            else
            {
                pdx = 0;
                pdy = incy;
                odx = incx;
                ody = incy;
                es = dx;
                el = dy;
            }

            // Init start
            int x = x1;
            int y = y1;
            int error = el >> 1;
            if (y < clipY2 && y >= clipY1 && x < clipX2 && x >= clipX1)
            {
                pixels[(y * w) + x] = color;
            }

            // Walk the line!
            for (int i = 0; i < el; i++)
            {
                // Update error term
                error -= es;

                // Decide which coord to use
                if (error < 0)
                {
                    error += el;
                    x += odx;
                    y += ody;
                }
                else
                {
                    x += pdx;
                    y += pdy;
                }

                // Set pixel
                if (y < clipY2 && y >= clipY1 && x < clipX2 && x >= clipX1)
                {
                    pixels[(y * w) + x] = color;
                }
            }
        }

        /// <summary>
        /// Draws a colored line by connecting two points using a DDA algorithm (Digital Differential Analyzer).
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
        /// <param name="clipRect">The region in the image to restrict drawing to.</param>
        public static void DrawLineDDA(this WriteableBitmap bmp, int x1, int y1, int x2, int y2, Color color, Rect? clipRect = null)
        {
            var col = ConvertColor(color);
            bmp.DrawLineDDA(x1, y1, x2, y2, col, clipRect);
        }

        /// <summary>
        /// Draws a colored line by connecting two points using a DDA algorithm (Digital Differential Analyzer).
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
        /// <param name="clipRect">The region in the image to restrict drawing to.</param>
        public static void DrawLineDDA(this WriteableBitmap bmp, int x1, int y1, int x2, int y2, int color, Rect? clipRect = null)
        {
            using var context = bmp.GetBitmapContext();
            // Use refs for faster access (really important!) speeds up a lot!
            int w = context.Width;
            int h = context.Height;
            var pixels = context.Pixels;

            // Get clip coordinates
            int clipX1 = 0;
            int clipX2 = w;
            int clipY1 = 0;
            int clipY2 = h;
            if (clipRect.HasValue)
            {
                var c = clipRect.Value;
                clipX1 = (int)c.X;
                clipX2 = (int)(c.X + c.Width);
                clipY1 = (int)c.Y;
                clipY2 = (int)(c.Y + c.Height);
            }

            // Distance start and end point
            int dx = x2 - x1;
            int dy = y2 - y1;

            // Determine slope (absolute value)
            int len = dy >= 0 ? dy : -dy;
            int lenx = dx >= 0 ? dx : -dx;
            if (lenx > len)
            {
                len = lenx;
            }

            // Prevent division by zero
            if (len != 0)
            {
                // Init steps and start
                float incx = dx / (float)len;
                float incy = dy / (float)len;
                float x = x1;
                float y = y1;

                // Walk the line!
                for (int i = 0; i < len; i++)
                {
                    if (y < clipY2 && y >= clipY1 && x < clipX2 && x >= clipX1)
                    {
                        pixels[((int)y * w) + (int)x] = color;
                    }
                    x += incx;
                    y += incy;
                }
            }
        }

        /// <summary>
        /// Draws a colored line by connecting two points using an optimized DDA.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
        /// <param name="clipRect">The region in the image to restrict drawing to.</param>
        public static void DrawLine(this WriteableBitmap bmp, int x1, int y1, int x2, int y2, Color color, Rect? clipRect = null)
        {
            var col = ConvertColor(color);
            bmp.DrawLine(x1, y1, x2, y2, col, clipRect);
        }

        /// <summary>
        /// Draws a colored line by connecting two points using an optimized DDA.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
        /// <param name="clipRect">The region in the image to restrict drawing to.</param>
        public static void DrawLine(this WriteableBitmap bmp, int x1, int y1, int x2, int y2, int color, Rect? clipRect = null)
        {
            using var context = bmp.GetBitmapContext();
            DrawLine(context, context.Width, context.Height, x1, y1, x2, y2, color, clipRect);
        }

        /// <summary>
        /// Draws a colored line by connecting two points using an optimized DDA.
        /// Uses the pixels array and the width directly for best performance.
        /// </summary>
        /// <param name="context">The context containing the pixels as int RGBA value.</param>
        /// <param name="pixelWidth">The width of one scanline in the pixels array.</param>
        /// <param name="pixelHeight">The height of the bitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
        /// <param name="clipRect">The region in the image to restrict drawing to.</param>
        public static void DrawLine(BitmapContext context, int pixelWidth, int pixelHeight, int x1, int y1, int x2, int y2, int color, Rect? clipRect = null)
        {
            // Get clip coordinates
            int clipX1 = 0;
            int clipX2 = pixelWidth;
            int clipY1 = 0;
            int clipY2 = pixelHeight;
            if (clipRect.HasValue)
            {
                var c = clipRect.Value;
                clipX1 = (int)c.X;
                clipX2 = (int)(c.X + c.Width);
                clipY1 = (int)c.Y;
                clipY2 = (int)(c.Y + c.Height);
            }

            // Perform cohen-sutherland clipping if either point is out of the viewport
            if (!CohenSutherlandLineClip(new Rect(clipX1, clipY1, clipX2 - clipX1, clipY2 - clipY1), ref x1, ref y1, ref x2, ref y2))
            {
                return;
            }

            var pixels = context.Pixels;

            // Distance start and end point
            int dx = x2 - x1;
            int dy = y2 - y1;

            const int PRECISION_SHIFT = 8;

            // Determine slope (absolute value)
            int lenX = dx >= 0 ? dx : -dx;
            int lenY = dy >= 0 ? dy : -dy;
            if (lenX > lenY)
            { // x increases by +/- 1
                if (dx < 0)
                {
                    (x2, x1) = (x1, x2);
                    (y2, y1) = (y1, y2);
                }

                // Init steps and start
                int incy = (dy << PRECISION_SHIFT) / dx;

                int y1s = y1 << PRECISION_SHIFT;
                int y2s = y2 << PRECISION_SHIFT;
                int hs = pixelHeight << PRECISION_SHIFT;

                if (y1 < y2)
                {
                    if (y1 >= clipY2 || y2 < clipY1)
                    {
                        return;
                    }
                    if (y1s < 0)
                    {
                        if (incy == 0)
                        {
                            return;
                        }
                        int oldy1s = y1s;
                        // Find lowest y1s that is greater or equal than 0.
                        y1s = incy - 1 + ((y1s + 1) % incy);
                        x1 += (y1s - oldy1s) / incy;
                    }
                    if (y2s >= hs)
                    {
                        if (incy != 0)
                        {
                            // Find highest y2s that is less or equal than ws - 1.
                            // y2s = y1s + n * incy. Find n.
                            y2s = hs - 1 - ((hs - 1 - y1s) % incy);
                            x2 = x1 + ((y2s - y1s) / incy);
                        }
                    }
                }
                else
                {
                    if (y2 >= clipY2 || y1 < clipY1)
                    {
                        return;
                    }
                    if (y1s >= hs)
                    {
                        if (incy == 0)
                        {
                            return;
                        }
                        int oldy1s = y1s;
                        // Find highest y1s that is less or equal than ws - 1.
                        // y1s = oldy1s + n * incy. Find n.
                        y1s = hs - 1 + (incy - ((hs - 1 - oldy1s) % incy));
                        x1 += (y1s - oldy1s) / incy;
                    }
                    if (y2s < 0)
                    {
                        if (incy != 0)
                        {
                            // Find lowest y2s that is greater or equal than 0.
                            // y2s = y1s + n * incy. Find n.
                            y2s = y1s % incy;
                            x2 = x1 + ((y2s - y1s) / incy);
                        }
                    }
                }

                if (x1 < 0)
                {
                    y1s -= incy * x1;
                    x1 = 0;
                }
                if (x2 >= pixelWidth)
                {
                    x2 = pixelWidth - 1;
                }

                int ys = y1s;

                // Walk the line!
                int y = ys >> PRECISION_SHIFT;
                int previousY = y;
                int index = x1 + (y * pixelWidth);
                int k = incy < 0 ? 1 - pixelWidth : 1 + pixelWidth;
                for (int x = x1; x <= x2; ++x)
                {
                    pixels[index] = color;
                    ys += incy;
                    y = ys >> PRECISION_SHIFT;
                    if (y != previousY)
                    {
                        previousY = y;
                        index += k;
                    }
                    else
                    {
                        ++index;
                    }
                }
            }
            else
            {
                // Prevent division by zero
                if (lenY == 0)
                {
                    return;
                }
                if (dy < 0)
                {
                    (x2, x1) = (x1, x2);
                    (y2, y1) = (y1, y2);
                }

                // Init steps and start
                int x1s = x1 << PRECISION_SHIFT;
                int x2s = x2 << PRECISION_SHIFT;
                int ws = pixelWidth << PRECISION_SHIFT;

                int incx = (dx << PRECISION_SHIFT) / dy;

                if (x1 < x2)
                {
                    if (x1 >= clipX2 || x2 < clipX1)
                    {
                        return;
                    }
                    if (x1s < 0)
                    {
                        if (incx == 0)
                        {
                            return;
                        }
                        int oldx1s = x1s;
                        // Find lowest x1s that is greater or equal than 0.
                        x1s = incx - 1 + ((x1s + 1) % incx);
                        y1 += (x1s - oldx1s) / incx;
                    }
                    if (x2s >= ws)
                    {
                        if (incx != 0)
                        {
                            // Find highest x2s that is less or equal than ws - 1.
                            // x2s = x1s + n * incx. Find n.
                            x2s = ws - 1 - ((ws - 1 - x1s) % incx);
                            y2 = y1 + ((x2s - x1s) / incx);
                        }
                    }
                }
                else
                {
                    if (x2 >= clipX2 || x1 < clipX1)
                    {
                        return;
                    }
                    if (x1s >= ws)
                    {
                        if (incx == 0)
                        {
                            return;
                        }
                        int oldx1s = x1s;
                        // Find highest x1s that is less or equal than ws - 1.
                        // x1s = oldx1s + n * incx. Find n.
                        x1s = ws - 1 + (incx - ((ws - 1 - oldx1s) % incx));
                        y1 += (x1s - oldx1s) / incx;
                    }
                    if (x2s < 0)
                    {
                        if (incx != 0)
                        {
                            // Find lowest x2s that is greater or equal than 0.
                            // x2s = x1s + n * incx. Find n.
                            x2s = x1s % incx;
                            y2 = y1 + ((x2s - x1s) / incx);
                        }
                    }
                }

                if (y1 < 0)
                {
                    x1s -= incx * y1;
                    y1 = 0;
                }
                if (y2 >= pixelHeight)
                {
                    y2 = pixelHeight - 1;
                }

                long index = x1s;
                int indexBaseValue = y1 * pixelWidth;

                // Walk the line!
                var inc = (pixelWidth << PRECISION_SHIFT) + incx;
                for (int y = y1; y <= y2; ++y)
                {
                    pixels[indexBaseValue + (index >> PRECISION_SHIFT)] = color;
                    index += inc;
                }
            }
        }
        #endregion

        #region Penned line

        /// <summary>
        /// Bitfields used to partition the space into 9 regions
        /// </summary>
        private const byte INSIDE = 0; // 0000
        private const byte LEFT = 1;   // 0001
        private const byte RIGHT = 2;  // 0010
        private const byte BOTTOM = 4; // 0100
        private const byte TOP = 8;    // 1000

        /// <summary>
        /// Draws a line using a pen / stamp for the line
        /// </summary>
        /// <param name="bmp">The WriteableBitmap containing the pixels as int RGBA value.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="penBmp">The pen bitmap.</param>
        /// <param name="clipRect">The region in the image to restrict drawing to.</param>
        public static void DrawLinePenned(this WriteableBitmap bmp, int x1, int y1, int x2, int y2, WriteableBitmap penBmp, Rect? clipRect = null)
        {
            using var context = bmp.GetBitmapContext();
            using var penContext = penBmp.GetBitmapContext(ReadWriteMode.ReadOnly);
            DrawLinePenned(context, context.Width, context.Height, x1, y1, x2, y2, penContext, clipRect);
        }

        /// <summary>
        /// Draws a line using a pen / stamp for the line
        /// </summary>
        /// <param name="context">The context containing the pixels as int RGBA value.</param>
        /// <param name="w">The width of one scanline in the pixels array.</param>
        /// <param name="h">The height of the bitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="pen">The pen context.</param>
        /// <param name="clipRect">The region in the image to restrict drawing to.</param>
        public static void DrawLinePenned(BitmapContext context, int w, int h, int x1, int y1, int x2, int y2, BitmapContext pen, Rect? clipRect = null)
        {
            // Edge case where lines that went out of vertical bounds clipped instead of disappearing
            if ((y1 < 0 && y2 < 0) || (y1 > h && y2 > h))
            {
                return;
            }

            if (x1 == x2 && y1 == y2)
            {
                return;
            }

            // Perform cohen-sutherland clipping if either point is out of the viewport
            if (!CohenSutherlandLineClip(clipRect ?? new Rect(0, 0, w, h), ref x1, ref y1, ref x2, ref y2))
            {
                return;
            }

            int size = pen.Width;
            int pw = size;
            var srcRect = new Rect(0, 0, size, size);

            // Distance start and end point
            int dx = x2 - x1;
            int dy = y2 - y1;

            // Determine sign for direction x
            int incx = 0;
            if (dx < 0)
            {
                dx = -dx;
                incx = -1;
            }
            else if (dx > 0)
            {
                incx = 1;
            }

            // Determine sign for direction y
            int incy = 0;
            if (dy < 0)
            {
                dy = -dy;
                incy = -1;
            }
            else if (dy > 0)
            {
                incy = 1;
            }

            // Which gradient is larger
            int pdx, pdy, odx, ody, es, el;
            if (dx > dy)
            {
                pdx = incx;
                pdy = 0;
                odx = incx;
                ody = incy;
                es = dy;
                el = dx;
            }
            else
            {
                pdx = 0;
                pdy = incy;
                odx = incx;
                ody = incy;
                es = dx;
                el = dy;
            }

            // Init start
            int x = x1;
            int y = y1;
            int error = el >> 1;

            var destRect = new Rect(x, y, size, size);

            if (y < h && y >= 0 && x < w && x >= 0)
            {
                //Blit(context.WriteableBitmap, new Rect(x,y,3,3), pen.WriteableBitmap, new Rect(0,0,3,3));
                Blit(context, w, h, destRect, pen, srcRect, pw);
                //pixels[y * w + x] = color;
            }

            // Walk the line!
            for (int i = 0; i < el; i++)
            {
                // Update error term
                error -= es;

                // Decide which coord to use
                if (error < 0)
                {
                    error += el;
                    x += odx;
                    y += ody;
                }
                else
                {
                    x += pdx;
                    y += pdy;
                }

                // Set pixel
                if (y < h && y >= 0 && x < w && x >= 0)
                {
                    //Blit(context, w, h, destRect, pen, srcRect, pw);
                    Blit(context, w, h, new Rect(x, y, size, size), pen, srcRect, pw);
                    //Blit(context.WriteableBitmap, destRect, pen.WriteableBitmap, srcRect);
                    //pixels[y * w + x] = color;
                }
            }
        }

        /// <summary>
        /// Compute the bit code for a point (x, y) using the clip rectangle
        /// bounded diagonally by (xmin, ymin), and (xmax, ymax)
        /// ASSUME THAT xmax , xmin , ymax and ymin are global constants.
        /// </summary>
        /// <param name="extents">The extents.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        private static byte ComputeOutCode(Rect extents, double x, double y)
        {
            // initialized as being inside of clip window
            byte code = INSIDE;

            if (x < extents.Left)           // to the left of clip window
            {
                code |= LEFT;
            }
            else if (x > extents.Right)     // to the right of clip window
            {
                code |= RIGHT;
            }

            if (y > extents.Bottom)         // below the clip window
            {
                code |= BOTTOM;
            }
            else if (y < extents.Top)       // above the clip window
            {
                code |= TOP;
            }

            return code;
        }

        #endregion

        #region Dotted Line
        /// <summary>
        /// Draws a colored dotted line
        /// </summary>
        /// <param name="bmp">The WriteableBitmap</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="dotSpace">length of space between each line segment</param>
        /// <param name="dotLength">length of each line segment</param>
        /// <param name="color">The color for the line.</param>
        public static void DrawLineDotted(this WriteableBitmap bmp, int x1, int y1, int x2, int y2, int dotSpace, int dotLength, Color color)
        {
            var c = ConvertColor(color);
            DrawLineDotted(bmp, x1, y1, x2, y2, dotSpace, dotLength, c);
        }
        /// <summary>
        /// Draws a colored dotted line
        /// </summary>
        /// <param name="bmp">The WriteableBitmap</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="dotSpace">length of space between each line segment</param>
        /// <param name="dotLength">length of each line segment</param>
        /// <param name="color">The color for the line.</param>
        public static void DrawLineDotted(this WriteableBitmap bmp, int x1, int y1, int x2, int y2, int dotSpace, int dotLength, int color)
        {
            //if (x1 == 0) {
            //    x1 = 1;
            //}
            //if (x2 == 0) {
            //    x2 = 1;
            //}
            //if (y1 == 0) {
            //    y1 = 1;
            //}
            //if (y2 == 0) {
            //    y2 = 1;
            //}
            //if (x1 < 1 || x2 < 1 || y1 < 1 || y2 < 1 || dotSpace < 1 || dotLength < 1) {
            //    throw new ArgumentOutOfRangeException("Value must be larger than 0");
            //}
            // vertically and horizontally checks by themselves if coords are out of bounds, otherwise CohenSutherlandCLip is used

            // vertically?
            using var context = bmp.GetBitmapContext();
            if (x1 == x2)
            {
                SwapHorV(ref y1, ref y2);
                DrawVertically(context, x1, y1, y2, dotSpace, dotLength, color);
            }
            // horizontally?
            else if (y1 == y2)
            {
                SwapHorV(ref x1, ref x2);
                DrawHorizontally(context, x1, x2, y1, dotSpace, dotLength, color);
            }
            else
            {
                Draw(context, x1, y1, x2, y2, dotSpace, dotLength, color);
            }
        }

        private static void DrawVertically(BitmapContext context, int x, int y1, int y2, int dotSpace, int dotLength, int color)
        {
            int width = context.Width;
            int height = context.Height;

            if (x < 0 || x > width)
            {
                return;
            }

            var pixels = context.Pixels;
            bool on = true;
            int spaceCnt = 0;
            for (int i = y1; i <= y2; i++)
            {
                if (i < 1)
                {
                    continue;
                }
                if (i >= height)
                {
                    break;
                }

                if (on)
                {
                    //bmp.SetPixel(x, i, color);
                    //var idx = GetIndex(x, i, width);
                    var idx = ((i - 1) * width) + x;
                    pixels[idx] = color;
                    on = i % dotLength != 0;
                    spaceCnt = 0;
                }
                else
                {
                    spaceCnt++;
                    on = spaceCnt % dotSpace == 0;
                }
                //System.Diagnostics.Debug.WriteLine($"{x},{i}, on = {on}");
            }
        }

        private static void DrawHorizontally(BitmapContext context, int x1, int x2, int y, int dotSpace, int dotLength, int color)
        {
            int width = context.Width;
            int height = context.Height;

            if (y < 0 || y > height)
            {
                return;
            }

            var pixels = context.Pixels;
            bool on = true;
            int spaceCnt = 0;
            for (int i = x1; i <= x2; i++)
            {
                if (i < 1)
                {
                    continue;
                }
                if (i >= width)
                {
                    break;
                }
                if (y >= height)
                {
                    break;
                }

                if (on)
                {
                    //bmp.SetPixel(i, y, color);
                    //var idx = GetIndex(i, y, width);
                    var idx = (y * width) + i - 1;
                    pixels[idx] = color;
                    on = i % dotLength != 0;
                    spaceCnt = 0;
                }
                else
                {
                    spaceCnt++;
                    on = spaceCnt % dotSpace == 0;
                }
                //System.Diagnostics.Debug.WriteLine($"{i},{y}, on = {on}");
            }
        }

        private static void Draw(BitmapContext context, int x1, int y1, int x2, int y2, int dotSpace, int dotLength, int color)
        {
            // y = m * x + n
            // y - m * x = n

            int width = context.Width;
            int height = context.Height;

            // Perform cohen-sutherland clipping if either point is out of the viewport
            if (!CohenSutherlandLineClip(new Rect(0, 0, width, height), ref x1, ref y1, ref x2, ref y2))
            {
                return;
            }
            Swap(ref x1, ref x2, ref y1, ref y2);
            float m = (y2 - y1) / (float)(x2 - x1);
            float n = y1 - (m * x1);
            var pixels = context.Pixels;

            bool on = true;
            int spaceCnt = 0;
            for (int i = x1; i <= width; i++)
            {
                if (i == 0)
                {
                    continue;
                }
                int y = (int)((m * i) + n);
                if (y <= 0)
                {
                    continue;
                }
                if (y >= height || i >= x2)
                {
                    continue;
                }
                if (on)
                {
                    //bmp.SetPixel(i, y, color);
                    //var idx = GetIndex(i, y, width);
                    var idx = ((y - 1) * width) + i - 1;
                    pixels[idx] = color;
                    spaceCnt = 0;
                    on = i % dotLength != 0;
                }
                else
                {
                    spaceCnt++;
                    on = spaceCnt % dotSpace == 0;
                }
            }
        }

        private static void Swap(ref int x1, ref int x2, ref int y1, ref int y2)
        {
            // always draw from left to right
            // or from top to bottom
            if (x2 < x1)
            {
                int tmpx1 = x1;
                int tmpx2 = x2;
                int tmpy1 = y1;
                int tmpy2 = y2;
                x1 = tmpx2;
                y1 = tmpy2;
                x2 = tmpx1;
                y2 = tmpy1;
            }
        }

        private static void SwapHorV(ref int a1, ref int a2)
        {
            int x1 = 0; // dummy
            int x2 = -1; // dummy
            if (a2 < a1)
            {
                Swap(ref x1, ref x2, ref a1, ref a2);
            }
        }

        // inlined
        //private static int GetIndex(int x, int y, int width) {
        //    var idx = (y - 1) * width + x;
        //    return idx - 1;
        //}
        #endregion

        #region Anti-alias line

        /// <summary>
        /// Draws an anti-aliased, alpha blended, colored line by connecting two points using Wu's antialiasing algorithm
        /// Uses the pixels array and the width directly for best performance.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x1">The x0.</param>
        /// <param name="y1">The y0.</param>
        /// <param name="x2">The x1.</param>
        /// <param name="y2">The y1.</param>
        /// <param name="sa">Alpha color component</param>
        /// <param name="sr">Premultiplied red color component</param>
        /// <param name="sg">Premultiplied green color component</param>
        /// <param name="sb">Premultiplied blue color component</param>
        /// <param name="clipRect">The region in the image to restrict drawing to.</param>
        public static void DrawLineWu(this WriteableBitmap bmp, int x1, int y1, int x2, int y2, int sa, int sr, int sg, int sb, Rect? clipRect = null)
        {
            using var context = bmp.GetBitmapContext();
            DrawLineWu(context, context.Width, context.Height, x1, y1, x2, y2, sa, sr, sg, sb, clipRect);
        }

        /// <summary>
        /// Draws an anti-aliased, alpha-blended, colored line by connecting two points using Wu's antialiasing algorithm
        /// Uses the pixels array and the width directly for best performance.
        /// </summary>
        /// <param name="context">An array containing the pixels as int RGBA value.</param>
        /// <param name="pixelWidth">The width of one scanline in the pixels array.</param>
        /// <param name="pixelHeight">The height of the bitmap.</param>
        /// <param name="x1">The x0.</param>
        /// <param name="y1">The y0.</param>
        /// <param name="x2">The x1.</param>
        /// <param name="y2">The y1.</param>
        /// <param name="sa">Alpha color component</param>
        /// <param name="sr">Premultiplied red color component</param>
        /// <param name="sg">Premultiplied green color component</param>
        /// <param name="sb">Premultiplied blue color component</param>
        /// <param name="clipRect">The region in the image to restrict drawing to.</param>
        public static void DrawLineWu(BitmapContext context, int pixelWidth, int pixelHeight, int x1, int y1, int x2, int y2, int sa, int sr, int sg, int sb, Rect? clipRect = null)
        {
            // Perform cohen-sutherland clipping if either point is out of the viewport
            if (!CohenSutherlandLineClip(clipRect ?? new Rect(0, 0, pixelWidth, pixelHeight), ref x1, ref y1, ref x2, ref y2))
            {
                return;
            }

            var pixels = context.Pixels;

            const ushort INTENSITY_BITS = 8;
            const short NUM_LEVELS = 1 << INTENSITY_BITS; // 256
            // mask used to compute 1-value by doing (value XOR mask)
            const ushort WEIGHT_COMPLEMENT_MASK = NUM_LEVELS - 1; // 255
            // # of bits by which to shift ErrorAcc to get intensity level
            const ushort INTENSITY_SHIFT = 16 - INTENSITY_BITS; // 8

            ushort ErrorAdj, ErrorAcc;
            ushort ErrorAccTemp, Weighting;
            short DeltaX, DeltaY, XDir;
            int tmp;
            // ensure line runs from top to bottom
            if (y1 > y2)
            {
                tmp = y1; y1 = y2; y2 = tmp;
                tmp = x1; x1 = x2; x2 = tmp;
            }

            // draw initial pixel, which is always intersected by line to it's at 100% intensity
            pixels[(y1 * pixelWidth) + x1] = AlphaBlend(sa, sr, sg, sb, pixels[(y1 * pixelWidth) + x1]);
            //bitmap.SetPixel(X0, Y0, BaseColor);

            DeltaX = (short)(x2 - x1);
            if (DeltaX >= 0)
            {
                XDir = 1;
            }
            else
            {
                XDir = -1;
                DeltaX = (short)-DeltaX; /* make DeltaX positive */
            }

            // Special-case horizontal, vertical, and diagonal lines, which
            // require no weighting because they go right through the center of
            // every pixel; also avoids division by zero later
            DeltaY = (short)(y2 - y1);
            if (DeltaY == 0) // if horizontal line
            {
                while (DeltaX-- != 0)
                {
                    x1 += XDir;
                    pixels[(y1 * pixelWidth) + x1] = AlphaBlend(sa, sr, sg, sb, pixels[(y1 * pixelWidth) + x1]);
                }
                return;
            }

            if (DeltaX == 0) // if vertical line
            {
                do
                {
                    y1++;
                    pixels[(y1 * pixelWidth) + x1] = AlphaBlend(sa, sr, sg, sb, pixels[(y1 * pixelWidth) + x1]);
                } while (--DeltaY != 0);
                return;
            }

            if (DeltaX == DeltaY) // diagonal line
            {
                do
                {
                    x1 += XDir;
                    y1++;
                    pixels[(y1 * pixelWidth) + x1] = AlphaBlend(sa, sr, sg, sb, pixels[(y1 * pixelWidth) + x1]);
                } while (--DeltaY != 0);
                return;
            }

            // Line is not horizontal, diagonal, or vertical
            ErrorAcc = 0;  // initialize the line error accumulator to 0

            // Is this an X-major or Y-major line?
            if (DeltaY > DeltaX)
            {
                // Y-major line; calculate 16-bit fixed-point fractional part of a
                // pixel that X advances each time Y advances 1 pixel, truncating the
                // result so that we won't overrun the endpoint along the X axis
                ErrorAdj = (ushort)(((ulong)DeltaX << 16) / (ulong)DeltaY);

                // Draw all pixels other than the first and last
                while (--DeltaY != 0)
                {
                    ErrorAccTemp = ErrorAcc;   // remember current accumulated error
                    ErrorAcc += ErrorAdj;      // calculate error for next pixel
                    if (ErrorAcc <= ErrorAccTemp)
                    {
                        // The error accumulator turned over, so advance the X coord */
                        x1 += XDir;
                    }
                    y1++; /* Y-major, so always advance Y */
                    // The IntensityBits most significant bits of ErrorAcc give us the
                    // intensity weighting for this pixel, and the complement of the
                    // weighting for the paired pixel
                    Weighting = (ushort)(ErrorAcc >> INTENSITY_SHIFT);

                    int weight = Weighting ^ WEIGHT_COMPLEMENT_MASK;
                    pixels[(y1 * pixelWidth) + x1] = AlphaBlend(sa, (sr * weight) >> 8, (sg * weight) >> 8, (sb * weight) >> 8, pixels[(y1 * pixelWidth) + x1]);

                    weight = Weighting;
                    pixels[(y1 * pixelWidth) + x1 + XDir] = AlphaBlend(sa, (sr * weight) >> 8, (sg * weight) >> 8, (sb * weight) >> 8, pixels[(y1 * pixelWidth) + x1 + XDir]);

                    //bitmap.SetPixel(X0, Y0, 255 - (BaseColor + Weighting));
                    //bitmap.SetPixel(X0 + XDir, Y0, 255 - (BaseColor + (Weighting ^ WeightingComplementMask)));
                }

                // Draw the final pixel, which is always exactly intersected by the line and so needs no weighting
                pixels[(y2 * pixelWidth) + x2] = AlphaBlend(sa, sr, sg, sb, pixels[(y2 * pixelWidth) + x2]);
                //bitmap.SetPixel(X1, Y1, BaseColor);
                return;
            }
            // It's an X-major line; calculate 16-bit fixed-point fractional part of a
            // pixel that Y advances each time X advances 1 pixel, truncating the
            // result to avoid overrunning the endpoint along the X axis */
            ErrorAdj = (ushort)(((ulong)DeltaY << 16) / (ulong)DeltaX);

            // Draw all pixels other than the first and last
            while (--DeltaX != 0)
            {
                ErrorAccTemp = ErrorAcc;   // remember current accumulated error
                ErrorAcc += ErrorAdj;      // calculate error for next pixel
                if (ErrorAcc <= ErrorAccTemp) // if error accumulator turned over
                {
                    // advance the Y coord
                    y1++;
                }
                x1 += XDir; // X-major, so always advance X
                // The IntensityBits most significant bits of ErrorAcc give us the
                // intensity weighting for this pixel, and the complement of the
                // weighting for the paired pixel
                Weighting = (ushort)(ErrorAcc >> INTENSITY_SHIFT);

                int weight = Weighting ^ WEIGHT_COMPLEMENT_MASK;
                pixels[(y1 * pixelWidth) + x1] = AlphaBlend(sa, (sr * weight) >> 8, (sg * weight) >> 8, (sb * weight) >> 8, pixels[(y1 * pixelWidth) + x1]);

                weight = Weighting;
                pixels[((y1 + 1) * pixelWidth) + x1] = AlphaBlend(sa, (sr * weight) >> 8, (sg * weight) >> 8, (sb * weight) >> 8, pixels[((y1 + 1) * pixelWidth) + x1]);

                //bitmap.SetPixel(X0, Y0, 255 - (BaseColor + Weighting));
                //bitmap.SetPixel(X0, Y0 + 1,
                //      255 - (BaseColor + (Weighting ^ WeightingComplementMask)));
            }
            // Draw the final pixel, which is always exactly intersected by the line and thus needs no weighting
            pixels[(y2 * pixelWidth) + x2] = AlphaBlend(sa, sr, sg, sb, pixels[(y2 * pixelWidth) + x2]);
            //bitmap.SetPixel(X1, Y1, BaseColor);
        }

        /// <summary>
        /// Draws an anti-aliased line with a desired stroke thickness
        /// <param name="context">The context containing the pixels as int RGBA value.</param>
        /// <param name="pixelWidth">The width of one scanline in the pixels array.</param>
        /// <param name="pixelHeight">The height of the bitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
        /// <param name="strokeThickness">The stroke thickness of the line.</param>
        /// <param name="clipRect">The region in the image to restrict drawing to.</param>
        /// </summary>
        public static void DrawLineAa(BitmapContext context, int pixelWidth, int pixelHeight, int x1, int y1, int x2, int y2, int color, int strokeThickness, Rect? clipRect = null)
        {
            AAWidthLine(pixelWidth, pixelHeight, context, x1, y1, x2, y2, strokeThickness, color, clipRect);
        }

        /// <summary>
        /// Draws an anti-aliased line with a desired stroke thickness
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
        /// <param name="strokeThickness">The stroke thickness of the line.</param>
        /// <param name="clipRect">The region in the image to restrict drawing to.</param>
        /// </summary>
        public static void DrawLineAa(this WriteableBitmap bmp, int x1, int y1, int x2, int y2, int color, int strokeThickness, Rect? clipRect = null)
        {
            using var context = bmp.GetBitmapContext();
            AAWidthLine(context.Width, context.Height, context, x1, y1, x2, y2, strokeThickness, color, clipRect);
        }

        /// <summary>
        /// Draws an anti-aliased line with a desired stroke thickness
        /// <param name="context">The context containing the pixels as int RGBA value.</param>
        /// <param name="pixelWidth">The width of one scanline in the pixels array.</param>
        /// <param name="pixelHeight">The height of the bitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
        /// <param name="strokeThickness">The stroke thickness of the line.</param>
        /// <param name="clipRect">The region in the image to restrict drawing to.</param>
        /// </summary>
        public static void DrawLineAa(BitmapContext context, int pixelWidth, int pixelHeight, int x1, int y1, int x2, int y2, Color color, int strokeThickness, Rect? clipRect = null)
        {
            var col = ConvertColor(color);
            AAWidthLine(pixelWidth, pixelHeight, context, x1, y1, x2, y2, strokeThickness, col, clipRect);
        }

        /// <summary>
        /// Draws an anti-aliased line with a desired stroke thickness
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
        /// <param name="strokeThickness">The stroke thickness of the line.</param>
        /// <param name="clipRect">The region in the image to restrict drawing to.</param>
        /// </summary>
        public static void DrawLineAa(this WriteableBitmap bmp, int x1, int y1, int x2, int y2, Color color, int strokeThickness, Rect? clipRect = null)
        {
            var col = ConvertColor(color);
            using var context = bmp.GetBitmapContext();
            AAWidthLine(context.Width, context.Height, context, x1, y1, x2, y2, strokeThickness, col, clipRect);
        }

        /// <summary>
        /// Draws an anti-aliased line, using an optimized version of Gupta-Sproull algorithm
        /// From http://nokola.com/blog/post/2010/10/14/Anti-aliased-Lines-And-Optimizing-Code-for-Windows-Phone-7e28093First-Look.aspx
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
        /// <param name="clipRect">The region in the image to restrict drawing to.</param>
        /// </summary>
        public static void DrawLineAa(this WriteableBitmap bmp, int x1, int y1, int x2, int y2, Color color, Rect? clipRect = null)
        {
            var col = ConvertColor(color);
            bmp.DrawLineAa(x1, y1, x2, y2, col, clipRect);
        }

        /// <summary>
        /// Draws an anti-aliased line, using an optimized version of Gupta-Sproull algorithm
        /// From http://nokola.com/blog/post/2010/10/14/Anti-aliased-Lines-And-Optimizing-Code-for-Windows-Phone-7e28093First-Look.aspx
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
        /// <param name="clipRect">The region in the image to restrict drawing to.</param>
        /// </summary>
        public static void DrawLineAa(this WriteableBitmap bmp, int x1, int y1, int x2, int y2, int color, Rect? clipRect = null)
        {
            using var context = bmp.GetBitmapContext();
            DrawLineAa(context, context.Width, context.Height, x1, y1, x2, y2, color, clipRect);
        }

        /// <summary>
        /// Draws an anti-aliased line, using an optimized version of Gupta-Sproull algorithm
        /// From http://nokola.com/blog/post/2010/10/14/Anti-aliased-Lines-And-Optimizing-Code-for-Windows-Phone-7e28093First-Look.aspx
        /// <param name="context">The context containing the pixels as int RGBA value.</param>
        /// <param name="pixelWidth">The width of one scanline in the pixels array.</param>
        /// <param name="pixelHeight">The height of the bitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
        /// <param name="clipRect">The region in the image to restrict drawing to.</param>
        /// </summary>
        public static void DrawLineAa(BitmapContext context, int pixelWidth, int pixelHeight, int x1, int y1, int x2, int y2, int color, Rect? clipRect = null)
        {
            if ((x1 == x2) && (y1 == y2))
            {
                return; // edge case causing invDFloat to overflow, found by Shai Rubinshtein
            }

            // Perform cohen-sutherland clipping if either point is out of the viewport
            if (!CohenSutherlandLineClip(clipRect ?? new Rect(0, 0, pixelWidth, pixelHeight), ref x1, ref y1, ref x2, ref y2))
            {
                return;
            }

            if (x1 < 1)
            {
                x1 = 1;
            }

            if (x1 > pixelWidth - 2)
            {
                x1 = pixelWidth - 2;
            }

            if (y1 < 1)
            {
                y1 = 1;
            }

            if (y1 > pixelHeight - 2)
            {
                y1 = pixelHeight - 2;
            }

            if (x2 < 1)
            {
                x2 = 1;
            }

            if (x2 > pixelWidth - 2)
            {
                x2 = pixelWidth - 2;
            }

            if (y2 < 1)
            {
                y2 = 1;
            }

            if (y2 > pixelHeight - 2)
            {
                y2 = pixelHeight - 2;
            }

            var addr = (y1 * pixelWidth) + x1;
            var dx = x2 - x1;
            var dy = y2 - y1;

            int du;
            int dv;
            int u;
            int v;
            int uincr;
            int vincr;

            // Extract color
            var a = (color >> 24) & 0xFF;
            var srb = (uint)(color & 0x00FF00FF);
            var sg = (uint)((color >> 8) & 0xFF);

            // By switching to (u,v), we combine all eight octants
            int adx = dx, ady = dy;
            if (dx < 0)
            {
                adx = -dx;
            }

            if (dy < 0)
            {
                ady = -dy;
            }

            if (adx > ady)
            {
                du = adx;
                dv = ady;
                u = x2;
                v = y2;
                uincr = 1;
                vincr = pixelWidth;
                if (dx < 0)
                {
                    uincr = -uincr;
                }

                if (dy < 0)
                {
                    vincr = -vincr;
                }
            }
            else
            {
                du = ady;
                dv = adx;
                u = y2;
                v = x2;
                uincr = pixelWidth;
                vincr = 1;
                if (dy < 0)
                {
                    uincr = -uincr;
                }

                if (dx < 0)
                {
                    vincr = -vincr;
                }
            }

            var uend = u + du;
            var d = (dv << 1) - du;        // Initial value as in Bresenham's
            var incrS = dv << 1;    // &#916;d for straight increments
            var incrD = (dv - du) << 1;    // &#916;d for diagonal increments

            var invDFloat = 1.0 / (4.0 * Math.Sqrt((du * du) + (dv * dv)));   // Precomputed inverse denominator
            var invD2DuFloat = 0.75 - (2.0 * (du * invDFloat));   // Precomputed constant

            const int PRECISION_SHIFT = 10; // result distance should be from 0 to 1 << PRECISION_SHIFT, mapping to a range of 0..1
            const int PRECISION_MULTIPLIER = 1 << PRECISION_SHIFT;
            var invD = (int)(invDFloat * PRECISION_MULTIPLIER);
            var invD2Du = (int)(invD2DuFloat * PRECISION_MULTIPLIER * a);
            var zeroDot75 = (int)(0.75 * PRECISION_MULTIPLIER * a);

            var invDMulAlpha = invD * a;
            var duMulInvD = du * invDMulAlpha; // used to help optimize twovdu * invD
            var dMulInvD = d * invDMulAlpha; // used to help optimize twovdu * invD
            //int twovdu = 0;    // Numerator of distance; starts at 0
            var twovduMulInvD = 0; // since twovdu == 0
            var incrSMulInvD = incrS * invDMulAlpha;
            var incrDMulInvD = incrD * invDMulAlpha;

            do
            {
                AlphaBlendNormalOnPremultiplied(context, addr, (zeroDot75 - twovduMulInvD) >> PRECISION_SHIFT, srb, sg);
                AlphaBlendNormalOnPremultiplied(context, addr + vincr, (invD2Du + twovduMulInvD) >> PRECISION_SHIFT, srb, sg);
                AlphaBlendNormalOnPremultiplied(context, addr - vincr, (invD2Du - twovduMulInvD) >> PRECISION_SHIFT, srb, sg);

                if (d < 0)
                {
                    // choose straight (u direction)
                    twovduMulInvD = dMulInvD + duMulInvD;
                    d += incrS;
                    dMulInvD += incrSMulInvD;
                }
                else
                {
                    // choose diagonal (u+v direction)
                    twovduMulInvD = dMulInvD - duMulInvD;
                    d += incrD;
                    dMulInvD += incrDMulInvD;
                    v++;
                    addr += vincr;
                }
                u++;
                addr += uincr;
            } while (u <= uend);
        }

        /// <summary>
        /// Blends a specific source color on top of a destination premultiplied color
        /// </summary>
        /// <param name="context">Array containing destination color</param>
        /// <param name="index">Index of destination pixel</param>
        /// <param name="sa">Source alpha (0..255)</param>
        /// <param name="srb">Source non-premultiplied red and blue component in the format 0x00rr00bb</param>
        /// <param name="sg">Source green component (0..255)</param>
        private static void AlphaBlendNormalOnPremultiplied(BitmapContext context, int index, int sa, uint srb, uint sg)
        {
            var pixels = context.Pixels;
            var destPixel = (uint)pixels[index];

            var da = destPixel >> 24;
            var dg = (destPixel >> 8) & 0xff;
            var drb = destPixel & 0x00FF00FF;

            // blend with high-quality alpha and lower quality but faster 1-off RGBs
            pixels[index] = (int)(
               ((sa + ((da * (255 - sa) * 0x8081) >> 23)) << 24) | // alpha
               ((((sg - dg) * sa) + (dg << 8)) & 0xFFFFFF00) | // green
               (((((srb - drb) * sa) >> 8) + drb) & 0x00FF00FF) // red and blue
            );
        }

        #endregion

        #region Helper

        internal static bool CohenSutherlandLineClipWithViewPortOffset(Rect viewPort, ref float xi0, ref float yi0, ref float xi1, ref float yi1, int offset)
        {
            var viewPortWithOffset = new Rect(viewPort.X - offset, viewPort.Y - offset, viewPort.Width + (2 * offset), viewPort.Height + (2 * offset));

            return CohenSutherlandLineClip(viewPortWithOffset, ref xi0, ref yi0, ref xi1, ref yi1);
        }

        public static bool CohenSutherlandLineClip(Rect extents, ref float xi0, ref float yi0, ref float xi1, ref float yi1)
        {
            // Fix #SC-1555: Log(0) issue
            // CohenSuzerland line clipping algorithm returns NaN when point has infinity value
            double x0 = ClipToInt(xi0);
            double y0 = ClipToInt(yi0);
            double x1 = ClipToInt(xi1);
            double y1 = ClipToInt(yi1);

            var isValid = CohenSutherlandLineClip(extents, ref x0, ref y0, ref x1, ref y1);

            // Update the clipped line
            xi0 = (float)x0;
            yi0 = (float)y0;
            xi1 = (float)x1;
            yi1 = (float)y1;

            return isValid;
        }

        private static float ClipToInt(float d)
        {
            return d > int.MaxValue ? int.MaxValue : d < int.MinValue ? int.MinValue : d;
        }

        public static bool CohenSutherlandLineClip(Rect extents, ref int xi0, ref int yi0, ref int xi1, ref int yi1)
        {
            double x0 = xi0;
            double y0 = yi0;
            double x1 = xi1;
            double y1 = yi1;

            var isValid = CohenSutherlandLineClip(extents, ref x0, ref y0, ref x1, ref y1);

            // Update the clipped line
            xi0 = (int)x0;
            yi0 = (int)y0;
            xi1 = (int)x1;
            yi1 = (int)y1;

            return isValid;
        }

        /// <summary>
        /// Cohen–Sutherland clipping algorithm clips a line from
        /// P0 = (x0, y0) to P1 = (x1, y1) against a rectangle with
        /// diagonal from (xmin, ymin) to (xmax, ymax).
        /// </summary>
        /// <remarks>See http://en.wikipedia.org/wiki/Cohen%E2%80%93Sutherland_algorithm for details</remarks>
        /// <returns>a list of two points in the resulting clipped line, or zero</returns>
        public static bool CohenSutherlandLineClip(Rect extents, ref double x0, ref double y0, ref double x1, ref double y1)
        {
            // compute outcodes for P0, P1, and whatever point lies outside the clip rectangle
            byte outcode0 = ComputeOutCode(extents, x0, y0);
            byte outcode1 = ComputeOutCode(extents, x1, y1);

            // No clipping if both points lie inside viewport
            if (outcode0 == INSIDE && outcode1 == INSIDE)
            {
                return true;
            }

            bool isValid = false;

            while (true)
            {
                // Bitwise OR is 0. Trivially accept and get out of loop
                if ((outcode0 | outcode1) == 0)
                {
                    isValid = true;
                    break;
                }
                // Bitwise AND is not 0. Trivially reject and get out of loop
                else if ((outcode0 & outcode1) != 0)
                {
                    break;
                }
                else
                {
                    // failed both tests, so calculate the line segment to clip
                    // from an outside point to an intersection with clip edge
                    double x, y;

                    // At least one endpoint is outside the clip rectangle; pick it.
                    byte outcodeOut = (outcode0 != 0) ? outcode0 : outcode1;

                    // Now find the intersection point;
                    // use formulas y = y0 + slope * (x - x0), x = x0 + (1 / slope) * (y - y0)
                    if ((outcodeOut & TOP) != 0)
                    {   // point is above the clip rectangle
                        x = x0 + ((x1 - x0) * (extents.Top - y0) / (y1 - y0));
                        y = extents.Top;
                    }
                    else if ((outcodeOut & BOTTOM) != 0)
                    { // point is below the clip rectangle
                        x = x0 + ((x1 - x0) * (extents.Bottom - y0) / (y1 - y0));
                        y = extents.Bottom;
                    }
                    else if ((outcodeOut & RIGHT) != 0)
                    {  // point is to the right of clip rectangle
                        y = y0 + ((y1 - y0) * (extents.Right - x0) / (x1 - x0));
                        x = extents.Right;
                    }
                    else if ((outcodeOut & LEFT) != 0)
                    {   // point is to the left of clip rectangle
                        y = y0 + ((y1 - y0) * (extents.Left - x0) / (x1 - x0));
                        x = extents.Left;
                    }
                    else
                    {
                        x = double.NaN;
                        y = double.NaN;
                    }

                    // Now we move outside point to intersection point to clip
                    // and get ready for next pass.
                    if (outcodeOut == outcode0)
                    {
                        x0 = x;
                        y0 = y;
                        outcode0 = ComputeOutCode(extents, x0, y0);
                    }
                    else
                    {
                        x1 = x;
                        y1 = y;
                        outcode1 = ComputeOutCode(extents, x1, y1);
                    }
                }
            }

            return isValid;
        }

        /// <summary>
        /// Alpha blends 2 premultiplied colors with each other
        /// </summary>
        /// <param name="sa">Source alpha color component</param>
        /// <param name="sr">Premultiplied source red color component</param>
        /// <param name="sg">Premultiplied source green color component</param>
        /// <param name="sb">Premultiplied source blue color component</param>
        /// <param name="destPixel">Premultiplied destination color</param>
        /// <returns>Premultiplied blended color value</returns>
        public static int AlphaBlend(int sa, int sr, int sg, int sb, int destPixel)
        {
            int dr, dg, db;
            int da;
            da = (destPixel >> 24) & 0xff;
            dr = (destPixel >> 16) & 0xff;
            dg = (destPixel >> 8) & 0xff;
            db = (destPixel) & 0xff;

            destPixel = ((sa + ((da * (255 - sa) * 0x8081) >> 23)) << 24) |
               ((sr + ((dr * (255 - sa) * 0x8081) >> 23)) << 16) |
               ((sg + ((dg * (255 - sa) * 0x8081) >> 23)) << 8) |
               (sb + ((db * (255 - sa) * 0x8081) >> 23));

            return destPixel;
        }

        #endregion
    }
}
