//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using SolidWorks.Interop.sldworks;
using System;
using System.ComponentModel;

namespace CodeStack.SwEx.MacroFeature.Mocks
{
    [Obsolete("Deprecated. Use DisplayDimensionPlaceholder instead")]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public class DisplayDimensionEmpty : Placeholders.DisplayDimensionPlaceholder
    {
    }
}

namespace CodeStack.SwEx.MacroFeature.Placeholders
{
    /// <summary>
    /// This is a mock implementation of display SOLIDWORKS dimension
    /// It is used in <see cref="Base.IParameterConverter.ConvertDisplayDimensions(IModelDoc2, IFeature, IDisplayDimension[])"/>
    /// for supporting the backward compatibility of macro feature parameters
    /// </summary>
    public class DisplayDimensionPlaceholder : DisplayDimension
    {
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool ArcExtensionLineOrOppositeSide { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int ArrowSide { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool BrokenLeader { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool CenterText { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int ChamferTextStyle { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool Diametric { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool DimensionToInside { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool DisplayAsChain { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool DisplayAsLinear { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool Elevation { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int EndSymbol { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool ExtensionLineExtendsFromCenterOfSet { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool ExtensionLineSameAsLeaderStyle { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool ExtensionLineUseDocumentDisplay { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool Foreshortened { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool GridBubble { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int HorizontalJustification { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool Inspection { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsLinked { get; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool Jogged { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int LeaderVisibility { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool LowerInspection { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool MarkedForDrawing { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double MaxWitnessLineLength { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool OffsetText { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool RunBidirectionally { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double Scale2 { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShortenedRadius { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShowDimensionValue { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShowLowerParenthesis { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShowParenthesis { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShowTolParenthesis { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool SmartWitness { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool SolidLeader { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool Split { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int Type2 { get; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int VerticalJustification { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int WitnessVisibility { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool AddDisplayEnt(int Type, object Data) { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool AddDisplayText(string Text, object Position, object Format, int Attachment, double WidthFactor) { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool AutoJogOrdinate() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool ExplementaryAngle() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetAlternatePrecision() { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetAlternatePrecision2() { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetAlternateTolPrecision() { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetAlternateTolPrecision2() { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object GetAnnotation() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetArcLengthLeader() { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetArrowHeadStyle() { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetArrowHeadStyle2(ref int Style1, ref int Style2) { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetAutoArcLengthLeader() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double GetBentLeaderLength() { return 0; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetBrokenLeader2() { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetChamferUnits(out int LengthUnit, out int AngularUnit) { LengthUnit = -1; AngularUnit = -1; return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public MathTransform GetDefinitionTransform() { return null; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object GetDimension() { return new DimensionPlaceholder(); }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public Dimension GetDimension2(int Index) { return new DimensionPlaceholder(); }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object GetDisplayData() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetExtensionLineAsCenterline(short ExtIndex, ref bool Centerline) { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetFractionBase() { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetFractionValue() { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object GetHoleCalloutVariables() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetJogParameters(short WitnessIndex, ref bool Jogged, ref double Offset1, ref double Offset2, ref double Offset1to2) { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string GetLinkedText() { return ""; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string GetLowerText() { return ""; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string GetNameForSelection() { return ""; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object GetNext() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object GetNext2() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object GetNext3() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public DisplayDimension GetNext4() { return null; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public DisplayDimension GetNext5() { return null; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public void GetOrdinateDimensionArrowSize(out bool UseDoc, out double ArrowSize) { UseDoc = false; ArrowSize = 0; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetOverride() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double GetOverrideValue() { return 0; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetPrimaryPrecision() { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetPrimaryPrecision2() { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetPrimaryTolPrecision() { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetPrimaryTolPrecision2() { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetRoundToFraction() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetSecondArrow() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetSupportsGenericText() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string GetText(int WhichText) { return ""; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object GetTextFormat() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetTextFormatItems(int WhichText, out object TokensDefinition, out object TokensEvaluated) { WhichText = -1; TokensDefinition = null; TokensEvaluated = null; return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetType() { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetUnits() { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetUseDocArrowHeadStyle() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetUseDocBentLeaderLength() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetUseDocBrokenLeader() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetUseDocDual() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetUseDocPrecision() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetUseDocSecondArrow() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetUseDocTextFormat() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetUseDocUnits() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetWitnessLineGap(short WitnessIndex, ref bool UseDoc, ref double Gap) { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int get_ChamferPrecision(int Index) { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool IAddDisplayEnt(int Type, ref double Data) { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool IAddDisplayText(string Text, ref double Position, TextFormat Format, int Attachment, double WidthFactor) { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public Annotation IGetAnnotation() { return null; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public Dimension IGetDimension() { return new DimensionPlaceholder(); }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public DisplayData IGetDisplayData() { return null; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public DisplayDimension IGetNext() { return null; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public DisplayDimension IGetNext2() { return null; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public DisplayDimension IGetNext3() { return null; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public TextFormat IGetTextFormat() { return null; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsDimXpert() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool ISetTextFormat(int TextFormatType, TextFormat TextFormat) { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsHoleCallout() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsReferenceDim() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool ResetExtensionLineStyle(short ExtIndex) { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int SetArcLengthLeader(bool AutoLeader, int LeaderType) { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public void SetArrowHeadStyle(bool UseDoc, int ArrowHeadStyle) { }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool SetArrowHeadStyle2(bool UseDoc, int Style1, int Style2) { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool SetBentLeaderLength(bool UseDoc, double Length) { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int SetBrokenLeader2(bool UseDoc, int Broken) { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public void SetDual(bool UseDoc) { }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public void SetDual2(bool UseDoc, bool InwardRounding) { }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool SetExtensionLineAsCenterline(short ExtIndex, bool Centerline) { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool SetJogParameters(short WitnessIndex, bool Jogged, double Offset1, double Offset2, double Offset1to2) { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool SetLineFontDimensionStyle(bool UseDoc, int Style) { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool SetLineFontDimensionThickness(bool UseDoc, int Style) { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool SetLineFontExtensionStyle(bool UseDoc, int Style) { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool SetLineFontExtensionThickness(bool UseDoc, int Style) { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int SetLinkedText(string BstrLinkedText) { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public void SetLowerText(string Text) { }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public void SetOrdinateDimensionArrowSize(bool UseDoc, double ArrowSize) { }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool SetOverride(bool Override, double Value) { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int SetPrecision(bool UseDoc, int Primary, int Alternate, int PrimaryTol, int AlternateTol) { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int SetPrecision2(int Primary, int Dual, int PrimaryTol, int DualTol) { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int SetPrecision3(int Primary, int Dual, int PrimaryTol, int DualTol) { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public void SetSecondArrow(bool UseDoc, bool SecondArrow) { }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public void SetText(int WhichText, string Text) { }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool SetTextFormat(int TextFormatType, object TextFormat) { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int SetUnits(bool UseDoc, int UType, int FractBase, int FractDenom, bool RoundToFraction) { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int SetUnits2(bool UseDoc, int UType, int FractBase, int FractDenom, bool RoundToFraction, int DecimalRounding) { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool SetWitnessLineGap(short WitnessIndex, bool UseDoc, double Gap) { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public void set_ChamferPrecision(int Index, int Precision) { }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool SupplementaryAngle() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int Unlink() { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool VerticallyOppositeAngle() { return false; }
    }
}
