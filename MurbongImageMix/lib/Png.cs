using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace Palc.Imaging
{
    /// <summary>
    /// Represents a PNG image and provides methods for editing its meta information
    /// </summary>
    public class Png : RawImage
    {
        private const int PAYLOAD_OFFSET = 8;
        private const int LENGTH_OF_LENGTHFIELD = 4;
        private const int LENGTH_OF_TYPEFIELD = 4;
        private const int LENGTH_OF_CHECKSUMFIELD = 4;
        private const int MINIMUM_CHUNK_LENGTH = LENGTH_OF_LENGTHFIELD + LENGTH_OF_TYPEFIELD + LENGTH_OF_CHECKSUMFIELD;

        public enum ChunkType
        {
            ImageData, PaletteTable, ImageTrailer, ImageHeader, Transparency,
            /*ColorSpace*/
            Chromaticities, Gamma, IccProfile, SignificantBits, RgbColorSpace,
            /*Text*/
            IsoText, CompressedText, UnicodeText,
            /*misc*/
            BackgroundColor, Histogram, PhyisicalDimensions, SuggestedPalette,
            Time,
            Unknown
        }

        /// <summary>
        /// Opens any image file and convert it to PNG. Load PNG data into memory.
        /// </summary>
        /// <param name="imagesource">The source image</param>
        public Png(Image imagesource)
            : base(imagesource)
        {

            if (imagesource == null)
            {
                throw new NullReferenceException("The image object passed is null");
            }

            // TODO implement image conversion to png
            throw new System.NotImplementedException("implicit conversion to PNG not possible yet");

        }

        /// <summary>
        /// load a PNG file
        /// </summary>
        /// <param name="sorucefilename"></param>
        public Png(string sourcefilename)
            : base(sourcefilename)
        {
            if (!HasValidPngHeader())
            {
                throw new System.ArgumentException("The file '" + sourcefilename + "' does not match PNG format criteria");
            }
        }

        /// <summary>
        /// Checks if the image data is in png file format
        /// </summary>
        private bool HasValidPngHeader()
        {
            int[] pngsignature = { 137, 80, 78, 71, 13, 10, 26, 10 };

            for (int i = 0; i < PAYLOAD_OFFSET; i++)
            {
                if (pngsignature[i] != (Int32)RawImageData[i])
                {
                    return false;
                }
            }
            return true;
        }

        // TODO Removing multiple chunks at a time would be useful

        /// <summary>
        /// Removes a chunk from the loaded PNG
        /// </summary>
        /// <param name="chunkBeingRemoved"></param>
        public void RemoveChunk(ChunkType chunkBeingRemoved)
        {
            int offset = PAYLOAD_OFFSET;
            int chunkDataLength = 0;
            int imageLength = RawImageData.Length;
            ChunkType chunkType = ChunkType.Unknown;
            int maxLengthToCheck = imageLength;//+ MINIMUM_CHUNK_LENGTH;

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(RawImageData, 0, offset);//png header

                while (offset < maxLengthToCheck)
                {
                    chunkDataLength = ReadChunkLength(offset);
                    chunkType = ReadChunkType(offset);
                    if (chunkType == ChunkType.ImageTrailer)
                    {
                        Debug.WriteLine("YES");
                    }
                    else
                    {
                        Debug.WriteLine("NO");
                    }
                    if (chunkType != chunkBeingRemoved)
                    {
                        ms.Write(RawImageData, offset, MINIMUM_CHUNK_LENGTH + chunkDataLength);
                        offset += MINIMUM_CHUNK_LENGTH + chunkDataLength;
                    }
                    else
                    {
                        // jump over the chunk we want to filter
                        offset += MINIMUM_CHUNK_LENGTH + chunkDataLength;
                    }
                }

                RawImageData = ms.ToArray();
            }

        }

        public void SetGammaChunk()
        {
            int offset = PAYLOAD_OFFSET;
            int chunkDataLength = 0;
            int imageLength = RawImageData.Length;
            ChunkType chunkType = ChunkType.Unknown;
            int maxLengthToCheck = imageLength;// - MINIMUM_CHUNK_LENGTH;

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(RawImageData, 0, offset);

                while (offset < maxLengthToCheck)
                {
                    chunkDataLength = ReadChunkLength(offset);
                    chunkType = ReadChunkType(offset);


                    if (chunkType == ChunkType.Gamma)
                    {
                        //ms.Write(RawImageData, offset, MINIMUM_CHUNK_LENGTH + chunkDataLength);
                        byte[] length = BitConverter.GetBytes(0x00000004);
                        byte[] gama = BitConverter.GetBytes(0x67414d41);
                        byte[] size = BitConverter.GetBytes(0x000008FC);
                        byte[] crc = BitConverter.GetBytes(0xF7F75472);
                        Array.Reverse(length);
                        Array.Reverse(gama);
                        Array.Reverse(size);
                        Array.Reverse(crc);
                        ms.Write(length, 0, 4);
                        ms.Write(gama, 0,4);
                        ms.Write(size, 0, 4);
                        ms.Write(crc, 0, 4);
                        offset += MINIMUM_CHUNK_LENGTH + chunkDataLength;
                    }
                    else
                    {
                        ms.Write(RawImageData, offset, MINIMUM_CHUNK_LENGTH + chunkDataLength);
                        offset += MINIMUM_CHUNK_LENGTH + chunkDataLength;
                    }
                }

                RawImageData = ms.ToArray();
            }

        }

        /// <summary>
        /// Gets the length data field length of the chunk starting at the offset. 
        /// </summary>
        /// <param name="offset">offset of chunk</param>
        /// <returns>length of chunk data</returns>
        private int ReadChunkLength(int offset)
        {
            byte[] chunkLength = new byte[LENGTH_OF_LENGTHFIELD];

            for (int i = 0; i < LENGTH_OF_LENGTHFIELD; i++)
            {
                chunkLength[i] = RawImageData[offset];
                offset++;
            }

            if (System.BitConverter.IsLittleEndian)
                System.Array.Reverse(chunkLength);

            return System.BitConverter.ToInt32(chunkLength, 0);

        }

        /// <summary>
        /// Gets the chunk type of the chunk beginning at the offset
        /// </summary>
        /// <param name="offset">offset of chunkoffset of chunk</param>
        /// <returns>type of chunk</returns>
        private ChunkType ReadChunkType(int offset)
        {
            string chunkType = System.Text.Encoding.ASCII.GetString(RawImageData, offset + LENGTH_OF_LENGTHFIELD, LENGTH_OF_TYPEFIELD);

            /* TODO improve according to clean code **/
            switch (chunkType)
            {
                case "gAMA":
                    return ChunkType.Gamma;

                case "IHDR":
                    return ChunkType.ImageHeader;

                case "IDAT":
                    return ChunkType.ImageData;

                case "IEND":
                    return ChunkType.ImageTrailer;

                case "PLTE":
                    return ChunkType.PaletteTable;

                case "tRNS":
                    return ChunkType.Transparency;

                case "cHRM":
                    return ChunkType.Chromaticities;

                case "iCCP":
                    return ChunkType.IccProfile;

                case "sBIT":
                    return ChunkType.SignificantBits;

                case "sRGB":
                    return ChunkType.RgbColorSpace;

                case "tEXt":
                    return ChunkType.IsoText;

                case "zTXt":
                    return ChunkType.CompressedText;

                case "iTXt":
                    return ChunkType.UnicodeText;

                case "bKGD":
                    return ChunkType.BackgroundColor;

                case "hIST":
                    return ChunkType.Histogram;

                case "pHYs":
                    return ChunkType.PhyisicalDimensions;

                case "sPLT":
                    return ChunkType.SuggestedPalette;

                case "tIME":
                    return ChunkType.Time;

                default:
                    return ChunkType.Unknown;
            }
        }
    }
}
