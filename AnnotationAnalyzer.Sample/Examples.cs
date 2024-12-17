// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace AnnotationAnalyzer.Sample;

public class Examples
{
    [NoManualSet] private int _blabla;

    public Examples() => _blabla = 5;

    public void SetBlabla(int value) => _blabla = value;
}

