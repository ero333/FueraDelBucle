using System;
using System.Collections.Generic;
using UnityEngine;

// Registra qué niveles se completaron.
// - En la build se guarda en PlayerPrefs: sobrevive a cerrar y abrir el juego.
// - En el Editor vive solo en memoria: dura lo que dura el Play y arranca
//   limpio cada vez que se le da Play de nuevo, así probar en Unity no deja
//   niveles marcados como completados.
public static class ProgresoNiveles
{
    private const string Clave = "NivelesCompletados";

#if UNITY_EDITOR
    private static HashSet<string> completadosEnMemoria = new HashSet<string>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ReiniciarAlEntrarEnPlay()
    {
        completadosEnMemoria = new HashSet<string>();
    }
#endif

    public static bool EstaCompletado(string nombreNivel)
    {
        return Leer().Contains(nombreNivel);
    }

    public static void MarcarCompletado(string nombreNivel)
    {
        HashSet<string> completados = Leer();

        if (!completados.Add(nombreNivel)) return;

#if !UNITY_EDITOR
        PlayerPrefs.SetString(Clave, string.Join(",", completados));
        PlayerPrefs.Save();
#endif
    }

    public static void BorrarProgreso()
    {
#if UNITY_EDITOR
        completadosEnMemoria = new HashSet<string>();
#endif
        PlayerPrefs.DeleteKey(Clave);
        PlayerPrefs.Save();
    }

    private static HashSet<string> Leer()
    {
#if UNITY_EDITOR
        return completadosEnMemoria;
#else
        string guardado = PlayerPrefs.GetString(Clave, "");
        return new HashSet<string>(guardado.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
#endif
    }
}
