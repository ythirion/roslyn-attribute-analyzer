using System;

namespace AnnotationAnalyzer.Sample;

[AttributeUsage(AttributeTargets.Field)]
public class NoManualSetAttribute : Attribute
{
}
