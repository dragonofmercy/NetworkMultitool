namespace NetworkMultitool.Geometry
{
    /// <summary>Outcome of a creation solver; port of the failure states of CS1 BaseCreateMode.CalcResult.</summary>
    public enum SolveResult
    {
        Ok,
        NotIntersect,
        WrongShape,
    }
}
