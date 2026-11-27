using NetworkMultitool.Tool;
using Unity.Entities;

namespace NetworkMultitool.Modes
{
    /// <summary>Ctrl+click on a segment picks the type (and upgrades, through the segment as template) of the road a mode creates. Shared by Parallel and the creation modes.</summary>
    public sealed class TypePicker
    {
        private Entity _hover;
        private Entity _edge;

        public bool Picked { get; private set; }

        public Entity Prefab { get; private set; }

        public string Name { get; private set; }

        /// <summary>Call every frame before anything snaps the hover to a node. True on the frame a segment was picked.</summary>
        public bool Update(ToolContext ctx)
        {
            _hover = ctx.CtrlHeld && ctx.HoverIsEdge && ctx.Em.Exists(ctx.Hover) && !ctx.Net.HasOwner(ctx.Hover) ? ctx.Hover : Entity.Null;
            if (!ctx.PickPressed || _hover == Entity.Null)
                return false;
            Picked = true;
            _edge = _hover;
            Prefab = ctx.Net.Prefab(_hover);
            Name = ctx.PrefabName(Prefab);
            return true;
        }

        public void Clear() => Picked = false;

        /// <summary>The picked segment while it exists, else <paramref name="fallback"/>.</summary>
        public Entity Template(ToolContext ctx, Entity fallback) => Picked && ctx.Em.Exists(_edge) ? _edge : fallback;

        public void Render(ToolContext ctx, Overlay overlay)
        {
            if (_hover != Entity.Null && ctx.Em.Exists(_hover))
                overlay.Edge(_hover, Overlay.Blue);
        }
    }
}
