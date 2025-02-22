﻿using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Avery16282Generator
{
    public static class TextSharpHelpers
    {
        public static void DrawRoundedRectangle(PdfContentByte canvas, Rectangle rectangle, BaseColor baseColor)
        {
            canvas.SetColorFill(baseColor);
            canvas.SetColorStroke(BaseColor.BLACK);
            canvas.RoundRectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, rectangle.Height, 5f);
            canvas.FillStroke();
        }

        public static void DrawRectangle(PdfContentByte canvas, Rectangle rectangle, BaseColor baseColor)
        {
            canvas.SetColorFill(baseColor);
            canvas.Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, rectangle.Height);
            canvas.Fill();
        }

        public static bool WriteWrappingTextInRectangle(PdfContentByte canvas, string text, Font font, Rectangle rectangle, int alignment, bool simulation = false)
        {
            var phrase = new Phrase(text, font);

            var columnText = new ColumnText(canvas)
            {
                Alignment = alignment
            };
            columnText.SetSimpleColumn(rectangle);
            columnText.AddElement(phrase);
            var result = columnText.Go(simulation);
            return !ColumnText.HasMoreText(result);
        }

        private static bool WriteNonWrappingTextInRectangle(PdfContentByte canvas, string text, Font font, Rectangle rectangle, int alignment, bool simulation = false)
        {
            var phrase = new Phrase(text, font);

            var columnText = new ColumnText(canvas)
            {
                Alignment = alignment
            };
            columnText.SetSimpleColumn(phrase, rectangle.Left, rectangle.Bottom, rectangle.Right, rectangle.Top, font.Size, alignment);
            var result = columnText.Go(simulation);
            return !ColumnText.HasMoreText(result);
        }

            public static float GetMultiLineFontSize(PdfContentByte canvas, string text, Rectangle rectangle, BaseFont baseFont, float maxFontSize, int alignment, int fontStyle)
        {
            var nextAttemptFontSize = maxFontSize;
            while (true)
            {
                var font = new Font(baseFont, nextAttemptFontSize, fontStyle, BaseColor.BLACK);
                if (WriteWrappingTextInRectangle(canvas, text, font, rectangle, alignment, true))
                {
                    return nextAttemptFontSize;
                }
                nextAttemptFontSize -= .2f;
            }
        }

        public static float GetFontSize(PdfContentByte canvas, string text, float width, BaseFont baseFont, float maxFontSize, int alignment, int fontStyle)
        {
            var nextAttemptFontSize = maxFontSize;
            var rectangle = new Rectangle(0, 0, width, nextAttemptFontSize * 1.2f);
            while (true)
            {
                var font = new Font(baseFont, nextAttemptFontSize, fontStyle, BaseColor.BLACK);
                if (WriteNonWrappingTextInRectangle(canvas, text, font, rectangle, alignment, true))
                {
                    return nextAttemptFontSize;
                }
                nextAttemptFontSize -= .2f;
                rectangle = new Rectangle(0, 0, width, nextAttemptFontSize * 1.2f);
            }
        }

        public static Image DrawImage(Rectangle rectangle, PdfContentByte canvas, string imagePath, float imageRotationInRadians, bool scaleAbsolute, bool centerVertically, bool centerHorizontally)
        {
            var image = Image.GetInstance(imagePath);
            image.Rotation = imageRotationInRadians;
            if (scaleAbsolute)
                image.ScaleAbsolute(rectangle.Rotate());
            else
                image.ScaleToFit(rectangle);
            var imageBottom = centerVertically ? rectangle.Bottom + (rectangle.Height - image.ScaledHeight) / 2 : rectangle.Bottom;
            var imageLeft = centerHorizontally ? rectangle.Left + (rectangle.Width - image.ScaledWidth) / 2 : rectangle.Left;
            image.SetAbsolutePosition(imageLeft, imageBottom);
            canvas.AddImage(image);
            return image;
        }

        public static void WriteCenteredNonWrappingTextInRectangle(PdfContentByte canvas, string text, Rectangle rectangle, Font font, int textRotation)
        {
            var x = rectangle.Left;
            var y = rectangle.Bottom + rectangle.Height/2;
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase(text, font),
                x, y,
                textRotation);
        }

        public static void WriteNonWrappingTextInRectangle(PdfContentByte canvas, string text, Rectangle rectangle, float textWidthOffset, float textHeightOffset, Font font, int textRotation)
        {
            var x = rectangle.Left + textWidthOffset;
            var y = rectangle.Top - textHeightOffset;
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(text, font),
                x, y,
                textRotation);
        }
    }
}
