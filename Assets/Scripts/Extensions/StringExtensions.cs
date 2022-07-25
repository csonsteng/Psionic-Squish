using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class StringExtensions
{
	public static bool IsNotNullOrEmpty(this string s) => !string.IsNullOrEmpty(s);


	public static bool IsNullOrEmpty(this string s) => string.IsNullOrEmpty(s);
}
