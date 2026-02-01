using System.Collections.Generic;
using UnityEngine;

public enum MaskPartType
{
    Hat,
    Ornament,
    Eyes // Escalable: Scar, Hat, Glasses, etc.
}
[System.Serializable]
public class MaskPartLibrary
{
    public MaskPartType type;
    public List<Sprite> sprites;
}

[System.Serializable]
public struct MaskPartId
{
    public MaskPartType type;
    public int index;
}
[System.Serializable]
public class MaskIdentity
{
    public Dictionary<MaskPartType, int> parts;
    public string ToDebugString()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        sb.Append("MaskIdentity { ");

        foreach (var part in parts)
        {
            sb.Append($"{part.Key}:{part.Value} ");
        }

        sb.Append("}");

        return sb.ToString();
    }
}

[System.Serializable]
public struct Clue
{
    public MaskPartId part;
    public Sprite sprite;
}
[System.Serializable]
public struct ClueDifficultyConfig
{
    public int falseCluesPerPart; // min 1, 2 o 3
}

public class GameController : MonoBehaviour
{
    [Header("NPCs")]
    [SerializeField] private MaskIdentity killerIdentity;
    [SerializeField] List<MaskIdentity> npcIdentities = new List<MaskIdentity>();

    [Header("NPCs Generation")]
    [SerializeField] private GameObject suspectPrefab;
    [SerializeField] private int npcCount = 100;
    [SerializeField] private float npcSpacing = 1.5f;
    [SerializeField] private Collider2D spawnAreaCollider;
    private List<SuspectHandler> spawnedSuspects = new List<SuspectHandler>();
    
    [Header("Clues")]
    [SerializeField] private ClueDifficultyConfig difficulty;
    [SerializeField] private List<Clue> clues;

    [Header("Sprite Resources")]
    [SerializeField] private List<MaskPartLibrary> maskPartLibraries;

    private void Start()
    {
        SpawnSuspects(npcCount);
        GenerateAllClues();
        AssignCluesToWitnesses();
    }

    // ========================= IDENTITY GENERATION ========================

    public MaskIdentity GenerateRandomIdentity()
    {
        var identity = new MaskIdentity
        {
            parts = new Dictionary<MaskPartType, int>()
        };

        foreach (var library in maskPartLibraries)
        {
            identity.parts[library.type] =
                Random.Range(0, library.sprites.Count);
        }

        return identity;
    }

    public bool AreIdentitiesEqual(MaskIdentity a, MaskIdentity b)
    {
        if (a.parts.Count != b.parts.Count)
            return false;

        foreach (var kvp in a.parts)
        {
            if (!b.parts.TryGetValue(kvp.Key, out int value))
                return false;

            if (value != kvp.Value)
                return false;
        }

        return true;
    }
    private void DebugIdentities()
    {
        Debug.Log($"KILLER → {killerIdentity.ToDebugString()}");

        /*
        for (int i = 0; i < npcIdentities.Count; i++)
        {
            Debug.Log($"NPC {i} → {npcIdentities[i].ToDebugString()}");
        }*/
    }

    // ========================= CLUES ========================

    private void GenerateAllClues()
    {
        clues = new List<Clue>();

        foreach (var part in killerIdentity.parts)
        {
            GenerateCluesForPart(
                part.Key,
                part.Value,
                difficulty.falseCluesPerPart
            );
        }
        Shuffle(clues);
    }

    private List<SuspectHandler> SelectWitnesses(int count)
    {
        List<SuspectHandler> candidates = new();

        foreach (var suspect in spawnedSuspects)
        {
            if (!suspect.isKiller)
                candidates.Add(suspect);
        }
        Shuffle(candidates);

        return candidates.GetRange(0, count);
    }
    private void AssignCluesToWitnesses()
    {
        int witnessCount = clues.Count;
        List<SuspectHandler> witnesses = SelectWitnesses(witnessCount);

        for (int i = 0; i < witnesses.Count; i++)
        {
            witnesses[i].SetAsWitness(clues[i]);
        }
    }

    private void GenerateCluesForPart(
    MaskPartType type,
    int killerIndex,
    int falseClueCount
)
    {
        var library = maskPartLibraries.Find(l => l.type == type);

        List<int> availableFalseIndexes = new List<int>();

        for (int i = 0; i < library.sprites.Count; i++)
        {
            if (i != killerIndex)
                availableFalseIndexes.Add(i);
        }

        Shuffle(availableFalseIndexes);

        // pistas falsas
        for (int i = 0; i < falseClueCount; i++)
        {
            int index = availableFalseIndexes[i];

            var partId = new MaskPartId
            {
                type = type,
                index = index
            };

            clues.Add(new Clue
            {
                part = partId,
                sprite = GetSprite(partId)
            });
        }

        // dos pistas correctas
        for (int i = 0; i < 2; i++)
        {
            var partId = new MaskPartId
            {
                type = type,
                index = killerIndex
            };

            clues.Add(new Clue
            {
                part = partId,
                sprite = GetSprite(partId)
            });
        }
    }


    // ========================= GAME STATES ========================

    public void OnPlayerInvestigate(Clue clue)
    {
        // Mostrar sprite en canvas
        Debug.Log($"INVESTIGATE → {clue.part.type}:{clue.part.index}");
    }
    public void OnPlayerAccuse(SuspectHandler suspect)
    {
        Debug.Log($"ACCUSE → {(suspect.isKiller ? "KILLER" : "WRONG")}");
        if (suspect.isKiller)
            OnPlayerWin();
        else
            OnPlayerLose();
    }

    private void OnPlayerWin()
    {
        Debug.Log("GANASTE");
    }

    private void OnPlayerLose()
    {
        Debug.Log("PERDISTE");
    }

    // ========================= INSTANTIATE NPCs ========================

    public void SpawnSuspects(int npcCount)
    {
        spawnedSuspects.Clear();

        // 1. Generar identidad del asesino
        killerIdentity = GenerateRandomIdentity();

        // 2. Generar identidades que no lo repitan
        npcIdentities.Clear();

        while (npcIdentities.Count < npcCount - 1)
        {
            MaskIdentity candidate = GenerateRandomIdentity();

            if (!AreIdentitiesEqual(candidate, killerIdentity))
            {
                npcIdentities.Add(candidate);
            }
        }

        // 3. Instanciar asesino (index 0)
        Vector2 killerPos = GetRandomPointInBounds(spawnAreaCollider.bounds);
        SpawnSingleSuspect(
            killerIdentity,
            true,
            killerPos
        );

        // 4. Instanciar NPCs normales
        for (int i = 0; i < npcIdentities.Count; i++)
        {
            Vector2 pos = GetRandomPointInBounds(spawnAreaCollider.bounds);

            SpawnSingleSuspect(
                npcIdentities[i],
                false,
                pos
            );
        }

        DebugIdentities();
    }

    private void SpawnSingleSuspect(
    MaskIdentity identity,
    bool isKiller,
    Vector3 position
)
    {
        GameObject instance = Instantiate(
            suspectPrefab,
            position,
            Quaternion.identity
        );

        SuspectHandler suspect = instance.GetComponent<SuspectHandler>();

        suspect.Initialize(
            this,
            identity,
            isKiller,
            position
        );

        spawnedSuspects.Add(suspect);
    }

    private Vector2 GetRandomPointInBounds(Bounds bounds)
    {
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector2(x, y);
    }

    // ========================= HELPERS ========================

    public Sprite GetSprite(MaskPartId partId)
    {
        var library = maskPartLibraries
            .Find(l => l.type == partId.type);

        return library.sprites[partId.index];
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

}
