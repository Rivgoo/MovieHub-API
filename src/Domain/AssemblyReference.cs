using System.Reflection;

namespace Domain;

/// <summary>
/// Provides a reference to the assembly containing the <see cref="AssemblyReference"/> type.
/// </summary>
public static class AssemblyReference
{
	/// <summary>
	/// Gets the <see cref="Assembly"/> instance for the assembly containing the <see cref="AssemblyReference"/> type.
	/// </summary>
	public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}