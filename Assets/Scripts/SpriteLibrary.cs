using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

/// <summary>
/// Procura sprites em Assets/Resources/&lt;pasta&gt;, ignorando maiúsculas, acentos, espaços e "_".
/// Ex.: SpriteLibrary.Find("Signatures", "TioCancoes")
/// </summary>
public static class SpriteLibrary
{
    private static readonly Dictionary<string, Dictionary<string, Sprite>> caches =
        new Dictionary<string, Dictionary<string, Sprite>>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetCache() => caches.Clear();

    private static Dictionary<string, Sprite> Get(string folder)
    {
        if (caches.TryGetValue(folder, out var dict)) return dict;

        dict = new Dictionary<string, Sprite>();
        var all = Resources.LoadAll<Sprite>(folder);

        // 1.ª passagem: nomes exatos (ex.: Azulhos, TioCancoes_2)
        foreach (var s in all)
        {
            string k = Normalize(s.name);
            if (!dict.ContainsKey(k)) dict[k] = s;
        }

        // 2.ª passagem: imagens importadas como "Multiple" ficam com o nome "Azulhos_0".
        // Aceita também o nome sem esse "_0" (só se esse nome ainda estiver livre).
        foreach (var s in all)
        {
            if (!s.name.EndsWith("_0")) continue;
            string k = Normalize(s.name.Substring(0, s.name.Length - 2));
            if (!dict.ContainsKey(k)) dict[k] = s;
        }

        caches[folder] = dict;
        return dict;
    }

    /// <summary>Devolve o sprite ou null se não existir.</summary>
    public static Sprite Find(string folder, string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        Get(folder).TryGetValue(Normalize(name), out var s);
        return s;
    }

    /// <summary>Lista dos nomes encontrados (para mensagens de ajuda).</summary>
    public static string Listing(string folder) => string.Join(", ", Get(folder).Keys);

    public static string Normalize(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";
        var decomposed = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (char c in decomposed)
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        return sb.ToString().ToLowerInvariant().Replace(" ", "").Replace("_", "").Replace("-", "");
    }
}
