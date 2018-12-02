using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.SwEx.MacroFeature.Mocks
{
    public class DisplayDimensionEmpty : DisplayDimension
    {
        public bool ArcExtensionLineOrOppositeSide { get; set; }
        public int ArrowSide { get; set; }
        public bool BrokenLeader { get; set; }
        public bool CenterText { get; set; }
        public int ChamferTextStyle { get; set; }
        public bool Diametric { get; set; }
        public bool DimensionToInside { get; set; }
        public bool DisplayAsChain { get; set; }
        public bool DisplayAsLinear { get; set; }
        public bool Elevation { get; set; }
        public int EndSymbol { get; set; }
        public bool ExtensionLineExtendsFromCenterOfSet { get; set; }
        public bool ExtensionLineSameAsLeaderStyle { get; set; }
        public bool ExtensionLineUseDocumentDisplay { get; set; }
        public bool Foreshortened { get; set; }
        public bool GridBubble { get; set; }
        public int HorizontalJustification { get; set; }
        public bool Inspection { get; set; }
        public bool IsLinked { get; }
        public bool Jogged { get; set; }
        public int LeaderVisibility { get; set; }
        public bool LowerInspection { get; set; }
        public bool MarkedForDrawing { get; set; }
        public double MaxWitnessLineLength { get; set; }
        public bool OffsetText { get; set; }
        public bool RunBidirectionally { get; set; }
        public double Scale2 { get; set; }
        public bool ShortenedRadius { get; set; }
        public bool ShowDimensionValue { get; set; }
        public bool ShowLowerParenthesis { get; set; }
        public bool ShowParenthesis { get; set; }
        public bool ShowTolParenthesis { get; set; }
        public bool SmartWitness { get; set; }
        public bool SolidLeader { get; set; }
        public bool Split { get; set; }
        public int Type2 { get; }
        public int VerticalJustification { get; set; }
        public int WitnessVisibility { get; set; }
        public bool AddDisplayEnt(int Type, object Data) { return false; }
        public bool AddDisplayText(string Text, object Position, object Format, int Attachment, double WidthFactor) { return false; }
        public bool AutoJogOrdinate() { return false; }
        public bool ExplementaryAngle() { return false; }
        public int GetAlternatePrecision() { return -1; }
        public int GetAlternatePrecision2() { return -1; }
        public int GetAlternateTolPrecision() { return -1; }
        public int GetAlternateTolPrecision2() { return -1; }
        public object GetAnnotation() { return false; }
        public int GetArcLengthLeader() { return -1; }
        public int GetArrowHeadStyle() { return -1; }
        public bool GetArrowHeadStyle2(ref int Style1, ref int Style2) { return false; }
        public bool GetAutoArcLengthLeader() { return false; }
        public double GetBentLeaderLength() { return 0; }
        public int GetBrokenLeader2() { return -1; }
        public bool GetChamferUnits(out int LengthUnit, out int AngularUnit) { LengthUnit = -1; AngularUnit = -1; return false; }
        public MathTransform GetDefinitionTransform() { return null; }
        public object GetDimension() { return new DimensionEmpty(); }
        public Dimension GetDimension2(int Index) { return new DimensionEmpty(); }
        public object GetDisplayData() { return false; }
        public bool GetExtensionLineAsCenterline(short ExtIndex, ref bool Centerline) { return false; }
        public int GetFractionBase() { return -1; }
        public int GetFractionValue() { return -1; }
        public object GetHoleCalloutVariables() { return false; }
        public bool GetJogParameters(short WitnessIndex, ref bool Jogged, ref double Offset1, ref double Offset2, ref double Offset1to2) { return false; }
        public string GetLinkedText() { return ""; }
        public string GetLowerText() { return ""; }
        public string GetNameForSelection() { return ""; }
        public object GetNext() { return false; }
        public object GetNext2() { return false; }
        public object GetNext3() { return false; }
        public DisplayDimension GetNext4() { return null; }
        public DisplayDimension GetNext5() { return null; }
        public void GetOrdinateDimensionArrowSize(out bool UseDoc, out double ArrowSize) { UseDoc = false; ArrowSize = 0; }
        public bool GetOverride() { return false; }
        public double GetOverrideValue() { return 0; }
        public int GetPrimaryPrecision() { return -1; }
        public int GetPrimaryPrecision2() { return -1; }
        public int GetPrimaryTolPrecision() { return -1; }
        public int GetPrimaryTolPrecision2() { return -1; }
        public bool GetRoundToFraction() { return false; }
        public bool GetSecondArrow() { return false; }
        public bool GetSupportsGenericText() { return false; }
        public string GetText(int WhichText) { return ""; }
        public object GetTextFormat() { return false; }
        public int GetTextFormatItems(int WhichText, out object TokensDefinition, out object TokensEvaluated) { WhichText = -1; TokensDefinition = null; TokensEvaluated = null; return -1; }
        public int GetType() { return -1; }
        public int GetUnits() { return -1; }
        public bool GetUseDocArrowHeadStyle() { return false; }
        public bool GetUseDocBentLeaderLength() { return false; }
        public bool GetUseDocBrokenLeader() { return false; }
        public bool GetUseDocDual() { return false; }
        public bool GetUseDocPrecision() { return false; }
        public bool GetUseDocSecondArrow() { return false; }
        public bool GetUseDocTextFormat() { return false; }
        public bool GetUseDocUnits() { return false; }
        public bool GetWitnessLineGap(short WitnessIndex, ref bool UseDoc, ref double Gap) { return false; }
        public int get_ChamferPrecision(int Index) { return -1; }
        public bool IAddDisplayEnt(int Type, ref double Data) { return false; }
        public bool IAddDisplayText(string Text, ref double Position, TextFormat Format, int Attachment, double WidthFactor) { return false; }
        public Annotation IGetAnnotation() { return null; }
        public Dimension IGetDimension() { return new DimensionEmpty(); }
        public DisplayData IGetDisplayData() { return null; }
        public DisplayDimension IGetNext() { return null; }
        public DisplayDimension IGetNext2() { return null; }
        public DisplayDimension IGetNext3() { return null; }
        public TextFormat IGetTextFormat() { return null; }
        public bool IsDimXpert() { return false; }
        public bool ISetTextFormat(int TextFormatType, TextFormat TextFormat) { return false; }
        public bool IsHoleCallout() { return false; }
        public bool IsReferenceDim() { return false; }
        public bool ResetExtensionLineStyle(short ExtIndex) { return false; }
        public int SetArcLengthLeader(bool AutoLeader, int LeaderType) { return -1; }
        public void SetArrowHeadStyle(bool UseDoc, int ArrowHeadStyle) { }
        public bool SetArrowHeadStyle2(bool UseDoc, int Style1, int Style2) { return false; }
        public bool SetBentLeaderLength(bool UseDoc, double Length) { return false; }
        public int SetBrokenLeader2(bool UseDoc, int Broken) { return -1; }
        public void SetDual(bool UseDoc) { }
        public void SetDual2(bool UseDoc, bool InwardRounding) { }
        public bool SetExtensionLineAsCenterline(short ExtIndex, bool Centerline) { return false; }
        public bool SetJogParameters(short WitnessIndex, bool Jogged, double Offset1, double Offset2, double Offset1to2) { return false; }
        public bool SetLineFontDimensionStyle(bool UseDoc, int Style) { return false; }
        public bool SetLineFontDimensionThickness(bool UseDoc, int Style) { return false; }
        public bool SetLineFontExtensionStyle(bool UseDoc, int Style) { return false; }
        public bool SetLineFontExtensionThickness(bool UseDoc, int Style) { return false; }
        public int SetLinkedText(string BstrLinkedText) { return -1; }
        public void SetLowerText(string Text) { }
        public void SetOrdinateDimensionArrowSize(bool UseDoc, double ArrowSize) { }
        public bool SetOverride(bool Override, double Value) { return false; }
        public int SetPrecision(bool UseDoc, int Primary, int Alternate, int PrimaryTol, int AlternateTol) { return -1; }
        public int SetPrecision2(int Primary, int Dual, int PrimaryTol, int DualTol) { return -1; }
        public int SetPrecision3(int Primary, int Dual, int PrimaryTol, int DualTol) { return -1; }
        public void SetSecondArrow(bool UseDoc, bool SecondArrow) { }
        public void SetText(int WhichText, string Text) { }
        public bool SetTextFormat(int TextFormatType, object TextFormat) { return false; }
        public int SetUnits(bool UseDoc, int UType, int FractBase, int FractDenom, bool RoundToFraction) { return -1; }
        public int SetUnits2(bool UseDoc, int UType, int FractBase, int FractDenom, bool RoundToFraction, int DecimalRounding) { return -1; }
        public bool SetWitnessLineGap(short WitnessIndex, bool UseDoc, double Gap) { return false; }
        public void set_ChamferPrecision(int Index, int Precision) { }
        public bool SupplementaryAngle() { return false; }
        public int Unlink() { return -1; }
        public bool VerticallyOppositeAngle() { return false; }
    }
}
