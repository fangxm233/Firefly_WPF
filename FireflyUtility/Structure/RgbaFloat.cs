using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace FireflyUtility.Structure
{
    public struct RgbaFloat : IEquatable<RgbaFloat>
    {
        private readonly Vector4 _channels;
        /// <summary>The total size, in bytes, of an RgbaFloat value.</summary>
        public static readonly int SizeInBytes = 16;
        /// <summary>Red (1, 0, 0, 1)</summary>
        public static readonly RgbaFloat Red = new RgbaFloat(1f, 0.0f, 0.0f, 1f);
        /// <summary>Dark Red (0.6f, 0, 0, 1)</summary>
        public static readonly RgbaFloat DarkRed = new RgbaFloat(0.6f, 0.0f, 0.0f, 1f);
        /// <summary>Green (0, 1, 0, 1)</summary>
        public static readonly RgbaFloat Green = new RgbaFloat(0.0f, 1f, 0.0f, 1f);
        /// <summary>Blue (0, 0, 1, 1)</summary>
        public static readonly RgbaFloat Blue = new RgbaFloat(0.0f, 0.0f, 1f, 1f);
        /// <summary>Yellow (1, 1, 0, 1)</summary>
        public static readonly RgbaFloat Yellow = new RgbaFloat(1f, 1f, 0.0f, 1f);
        /// <summary>Grey (0.25f, 0.25f, 0.25f, 1)</summary>
        public static readonly RgbaFloat Grey = new RgbaFloat(0.25f, 0.25f, 0.25f, 1f);
        /// <summary>Light Grey (0.65f, 0.65f, 0.65f, 1)</summary>
        public static readonly RgbaFloat LightGrey = new RgbaFloat(0.65f, 0.65f, 0.65f, 1f);
        /// <summary>Cyan (0, 1, 1, 1)</summary>
        public static readonly RgbaFloat Cyan = new RgbaFloat(0.0f, 1f, 1f, 1f);
        /// <summary>White (1, 1, 1, 1)</summary>
        public static readonly RgbaFloat White = new RgbaFloat(1f, 1f, 1f, 1f);
        /// <summary>Cornflower Blue (0.3921f, 0.5843f, 0.9294f, 1)</summary>
        public static readonly RgbaFloat CornflowerBlue = new RgbaFloat(0.3921f, 0.5843f, 0.9294f, 1f);
        /// <summary>Clear (0, 0, 0, 0)</summary>
        public static readonly RgbaFloat Clear = new RgbaFloat(0.0f, 0.0f, 0.0f, 0.0f);
        /// <summary>Black (0, 0, 0, 1)</summary>
        public static readonly RgbaFloat Black = new RgbaFloat(0.0f, 0.0f, 0.0f, 1f);
        /// <summary>Pink (1, 0.45f, 0.75f, 1)</summary>
        public static readonly RgbaFloat Pink = new RgbaFloat(1f, 0.45f, 0.75f, 1f);
        /// <summary>Orange (1, 0.36f, 0, 1)</summary>
        public static readonly RgbaFloat Orange = new RgbaFloat(1f, 0.36f, 0.0f, 1f);

        /// <summary>The red component.</summary>
        public float R => this._channels.X;

        /// <summary>The green component.</summary>
        public float G => this._channels.Y;

        /// <summary>The blue component.</summary>
        public float B => this._channels.Z;

        /// <summary>The alpha component.</summary>
        public float A => this._channels.W;

        /// <summary>Constructs a new RgbaFloat from the given components.</summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="a">The alpha component.</param>
        public RgbaFloat(float r, float g, float b, float a) => this._channels = new Vector4(r, g, b, a);

        /// <summary>
        /// Constructs a new RgbaFloat from the XYZW components of a vector.
        /// </summary>
        /// <param name="channels">The vector containing the color components.</param>
        public RgbaFloat(Vector4 channels) => this._channels = channels;

        /// <summary>Converts this RgbaFloat into a Vector4.</summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector4 ToVector4() => this._channels;

        /// <summary>Element-wise equality.</summary>
        /// <param name="other">The instance to compare to.</param>
        /// <returns>True if all elements are equal; false otherswise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(RgbaFloat other) => this._channels.Equals(other._channels);

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj) => obj is RgbaFloat other && this.Equals(other);

        /// <summary>Returns a string representation of this color.</summary>
        /// <returns></returns>
        public override string ToString() => string.Format("R:{0}, G:{1}, B:{2}, A:{3}", (object)this.R, (object)this.G, (object)this.B, (object)this.A);

        /// <summary>Element-wise equality.</summary>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(RgbaFloat left, RgbaFloat right) => left.Equals(right);

        /// <summary>Element-wise inequality.</summary>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(RgbaFloat left, RgbaFloat right) => !left.Equals(right);
    }
}
