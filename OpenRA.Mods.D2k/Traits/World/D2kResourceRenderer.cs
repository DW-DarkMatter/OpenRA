#region Copyright & License Information
/*
 * Copyright 2007-2020 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using OpenRA.Mods.Common.Traits;

namespace OpenRA.Mods.D2k.Traits
{
	[Desc("Used to render spice with round borders.", "Attach this to the world actor")]
	public class D2kResourceRendererInfo : ResourceRendererInfo
	{
		public override object Create(ActorInitializer init) { return new D2kResourceRenderer(init.Self, this); }
	}

	public class D2kResourceRenderer : ResourceRenderer
	{
		[Flags]
		public enum ClearSides : byte
		{
			None = 0x0,
			Left = 0x1,
			Top = 0x2,
			Right = 0x4,
			Bottom = 0x8,

			TopLeft = 0x10,
			TopRight = 0x20,
			BottomLeft = 0x40,
			BottomRight = 0x80,

			All = 0xFF
		}

		public static readonly Dictionary<string, int[]> Variants = new Dictionary<string, int[]>()
		{
			{ "cleara", new[] { 0, 50 } },
			{ "clearb", new[] { 1, 51 } },
			{ "clearc", new[] { 43, 52 } },
			{ "cleard", new[] { 0, 53 } },
		};

		public static readonly Dictionary<ClearSides, int> SpriteMap = new Dictionary<ClearSides, int>()
		{
			{ ClearSides.None, 0 },
			{ ClearSides.Left | ClearSides.Top | ClearSides.TopLeft | ClearSides.TopRight | ClearSides.BottomLeft | ClearSides.BottomRight, 2 },
			{ ClearSides.Top | ClearSides.Right | ClearSides.TopLeft | ClearSides.TopRight | ClearSides.BottomLeft | ClearSides.BottomRight, 3 },
			{ ClearSides.Left | ClearSides.Bottom | ClearSides.TopLeft | ClearSides.TopRight | ClearSides.BottomLeft | ClearSides.BottomRight, 4 },
			{ ClearSides.Right | ClearSides.Bottom | ClearSides.TopLeft | ClearSides.TopRight | ClearSides.BottomLeft | ClearSides.BottomRight, 5 },
			{ ClearSides.Left | ClearSides.TopLeft | ClearSides.TopRight | ClearSides.BottomLeft | ClearSides.BottomRight, 6 },
			{ ClearSides.Right | ClearSides.TopLeft | ClearSides.TopRight | ClearSides.BottomLeft | ClearSides.BottomRight, 7 },
			{ ClearSides.Top | ClearSides.TopLeft | ClearSides.TopRight | ClearSides.BottomLeft | ClearSides.BottomRight, 8 },
			{ ClearSides.Bottom | ClearSides.TopLeft | ClearSides.TopRight | ClearSides.BottomLeft | ClearSides.BottomRight, 9 },
			{ ClearSides.Left | ClearSides.Top | ClearSides.TopLeft | ClearSides.TopRight | ClearSides.BottomLeft, 10 },
			{ ClearSides.Top | ClearSides.Right | ClearSides.TopLeft | ClearSides.TopRight | ClearSides.BottomRight, 11 },
			{ ClearSides.Left | ClearSides.Bottom | ClearSides.TopLeft | ClearSides.BottomLeft | ClearSides.BottomRight, 12 },
			{ ClearSides.Right | ClearSides.Bottom | ClearSides.TopRight | ClearSides.BottomLeft | ClearSides.BottomRight, 13 },
			{ ClearSides.Left | ClearSides.Top | ClearSides.Right | ClearSides.TopLeft | ClearSides.TopRight | ClearSides.BottomLeft | ClearSides.BottomRight, 14 },
			{ ClearSides.Left | ClearSides.Right | ClearSides.Bottom | ClearSides.TopLeft | ClearSides.TopRight | ClearSides.BottomLeft | ClearSides.BottomRight, 15 },
			{ ClearSides.Left | ClearSides.Top | ClearSides.Bottom | ClearSides.TopLeft | ClearSides.TopRight | ClearSides.BottomLeft | ClearSides.BottomRight, 16 },
			{ ClearSides.Top | ClearSides.Right | ClearSides.Bottom | ClearSides.TopLeft | ClearSides.TopRight | ClearSides.BottomLeft | ClearSides.BottomRight, 17 },
			{ ClearSides.Top | ClearSides.TopLeft | ClearSides.TopRight, 18 },
			{ ClearSides.Right | ClearSides.TopRight | ClearSides.BottomRight, 19 },
			{ ClearSides.Left | ClearSides.TopLeft | ClearSides.BottomLeft, 20 },
			{ ClearSides.Bottom | ClearSides.BottomLeft | ClearSides.BottomRight, 21 },
			{ ClearSides.TopLeft, 22 },
			{ ClearSides.TopRight, 23 },
			{ ClearSides.BottomLeft, 24 },
			{ ClearSides.BottomRight, 25 },
			{ ClearSides.Left | ClearSides.TopLeft | ClearSides.BottomLeft | ClearSides.BottomRight, 26 },
			{ ClearSides.Right | ClearSides.TopRight | ClearSides.BottomLeft | ClearSides.BottomRight, 27 },
			{ ClearSides.Top | ClearSides.TopLeft | ClearSides.TopRight | ClearSides.BottomRight, 28 },
			{ ClearSides.Top | ClearSides.TopLeft | ClearSides.TopRight | ClearSides.BottomLeft, 29 },
			{ ClearSides.TopRight | ClearSides.BottomLeft | ClearSides.BottomRight, 30 },
			{ ClearSides.TopLeft | ClearSides.BottomLeft | ClearSides.BottomRight, 31 },
			{ ClearSides.TopLeft | ClearSides.TopRight | ClearSides.BottomRight, 32 },
			{ ClearSides.TopLeft | ClearSides.TopRight | ClearSides.BottomLeft, 33 },
			{ ClearSides.TopRight | ClearSides.BottomRight, 34 },
			{ ClearSides.TopLeft | ClearSides.TopRight, 35 },
			{ ClearSides.TopRight | ClearSides.BottomLeft, 36 },
			{ ClearSides.TopLeft | ClearSides.BottomLeft, 37 },
			{ ClearSides.BottomLeft | ClearSides.BottomRight, 38 },
			{ ClearSides.TopLeft | ClearSides.BottomRight, 39 },
			{ ClearSides.TopLeft | ClearSides.TopRight | ClearSides.BottomLeft | ClearSides.BottomRight, 40 },
			{ ClearSides.Left | ClearSides.Right | ClearSides.TopLeft | ClearSides.TopRight | ClearSides.BottomLeft | ClearSides.BottomRight, 41 },
			{ ClearSides.Top | ClearSides.Bottom | ClearSides.TopLeft | ClearSides.TopRight | ClearSides.BottomLeft | ClearSides.BottomRight, 42 },
			{ ClearSides.All, 44 },
			{ ClearSides.Left | ClearSides.TopLeft | ClearSides.TopRight | ClearSides.BottomLeft, 46 },
			{ ClearSides.Right | ClearSides.TopLeft | ClearSides.TopRight | ClearSides.BottomRight, 47 },
			{ ClearSides.Bottom | ClearSides.TopRight | ClearSides.BottomLeft | ClearSides.BottomRight, 48 },
			{ ClearSides.Bottom | ClearSides.TopLeft | ClearSides.BottomLeft | ClearSides.BottomRight, 49 },
		};

		public D2kResourceRenderer(Actor self, D2kResourceRendererInfo info)
			: base(self, info) { }

		bool CellContains(CPos c, ResourceType t)
		{
			return RenderContent.Contains(c) && RenderContent[c].Type == t;
		}

		ClearSides FindClearSides(ResourceType t, CPos p)
		{
			var ret = ClearSides.None;
			if (!CellContains(p + new CVec(0, -1), t))
				ret |= ClearSides.Top | ClearSides.TopLeft | ClearSides.TopRight;

			if (!CellContains(p + new CVec(-1, 0), t))
				ret |= ClearSides.Left | ClearSides.TopLeft | ClearSides.BottomLeft;

			if (!CellContains(p + new CVec(1, 0), t))
				ret |= ClearSides.Right | ClearSides.TopRight | ClearSides.BottomRight;

			if (!CellContains(p + new CVec(0, 1), t))
				ret |= ClearSides.Bottom | ClearSides.BottomLeft | ClearSides.BottomRight;

			if (!CellContains(p + new CVec(-1, -1), t))
				ret |= ClearSides.TopLeft;

			if (!CellContains(p + new CVec(1, -1), t))
				ret |= ClearSides.TopRight;

			if (!CellContains(p + new CVec(-1, 1), t))
				ret |= ClearSides.BottomLeft;

			if (!CellContains(p + new CVec(1, 1), t))
				ret |= ClearSides.BottomRight;

			return ret;
		}

		protected override void UpdateRenderedSprite(CPos cell, RendererCellContents content)
		{
			UpdateRenderedSpriteInner(cell, content);

			var directions = CVec.Directions;
			for (var i = 0; i < directions.Length; i++)
				UpdateRenderedSpriteInner(cell + directions[i]);
		}

		void UpdateRenderedSpriteInner(CPos cell)
		{
			UpdateRenderedSpriteInner(cell, RenderContent[cell]);
		}

		void UpdateRenderedSpriteInner(CPos cell, RendererCellContents content)
		{
			var density = content.Density;
			var renderType = content.Type;

			if (density > 0 && renderType != null)
			{
				// The call chain for this method (that starts with AddDirtyCell()) guarantees
				// that the new content type would still be suitable for this renderer,
				// but that is a bit too fragile to rely on in case the code starts changing.
				if (!Info.RenderTypes.Contains(renderType.Info.Type))
					return;

				var clear = FindClearSides(renderType, cell);
				int index;

				if (clear == ClearSides.None)
				{
					var sprites = Variants[content.Variant];
					var frame = density > ResourceLayer.GetMaxResourceDensity(cell) / 2 ? 1 : 0;

					UpdateSpriteLayers(cell, renderType.Variants.First().Value[sprites[frame]], renderType.Palette);
				}
				else if (SpriteMap.TryGetValue(clear, out index))
				{
					UpdateSpriteLayers(cell, renderType.Variants.First().Value[index], renderType.Palette);
				}
				else
					throw new InvalidOperationException("SpriteMap does not contain an index for ClearSides type '{0}'".F(clear));
			}
			else
				UpdateSpriteLayers(cell, null, null);
		}

		protected override string ChooseRandomVariant(ResourceType t)
		{
			return Variants.Keys.Random(Game.CosmeticRandom);
		}
	}
}
