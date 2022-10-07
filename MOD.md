# WIKI: MODIFICANDO CITY SKYLINES

Date: October 7, 2022

# MODDING BASICS

## UNITY

El juego fue desarrollado con el game engine de Unity. Lo que significa que tienes acceso a todas las funciones que ofrece Unity al momento de crear mods.

### MODDING API

Para conocer todo lo que es posible hacer con mods, revisa la documentación del modding API en este enlace [https://skylines.paradoxwikis.com/Modding_API](https://skylines.paradoxwikis.com/Modding_API).

### NAMESPACES

Es recomendado utilizar namespaces para evitar conflictos con más mods.

### USING DIRECTIVES

Para poder utilizar el API de City Skylines y otros assemblies, es necesario usar directivos. Los que siempre estarán en el mods son los siguientes

```csharp
using ICities;
using UnityEngine;
```

### IUSERMOD

Al implementar `**IUserMod**`, es necesario especificar el nombre y descripción del mod. Estos es para que el Content Manager pueda mostrar tu mod. Código de ejemplo:

```csharp
using ICities;
using UnityEngine;

namespace ModName
{
	public class ModName : IUserMod
	{
		public string Name { get { return "Mod Name"; }}
		public string Description { get { return "Mod Description ..."; }}}
	}
}
```

## MOD BÁSICO

Para los mods básicos, solo se estarán implementando interfaces y extendiendo las clases bases del juego. Este nuevo código puede ser añadido a el main file o un nuevo archivo.

```csharp
public class UnlimitedOilAndOreResource : ResourceExtensionBase
{
	public override void OnAfterResourcesModified(int x, int z,
																											NaturalResource type,
																											int amount)
	{
		if ((type == NaturalResource.Oil || type == NaturalResource.Ore)
			&& amount < 0)
		{
			resourceManager.SetResource(x, z, type,
				(byte)(resourceManager.GetResource(x, z, type) - amount),
				false);
		}
	}
}
```

## ADVANCED MODS

Se utiliza ingeniería inversa del código en C# para descubrir exactamente que es lo que se puede hacer. También se puede usar la técnica de reflection para ir aún más lejos, pero con un grado de dificultad mayor.
