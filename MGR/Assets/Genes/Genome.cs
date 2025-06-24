using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genome : MonoBehaviour {
    public float[] genes;
    public int geneLength = 10;

    //[tê¿yzna, popêd, zwinnoœæ , atrakcyjnoœæ, percepcja , si³a]
    // tê¿yzna wp³ywa na move speed, i zmniejszenie wzrostu g³odu i pragnienia
    // popêd wp³ywa na to jak czêsto mo¿e siê dany osobnik rozmna¿aæ, dla samicy oznacza gotowoœæ do godów
    // zwinnoœæ wp³ywa na prêdkoœæ i szansê uniku
    // szansa na akceptacjê samca przez samicê
    // percepcja wp³ywa na zasiêg wykrywania


    private void Start() {
        InitializeRandom();
    }

    // Inicjalizacja genomu losowymi wartoœciami (0-1)
    public void InitializeRandom() {
        genes = new float[geneLength];
        for (int i = 0; i < geneLength; i++) {
            genes[i] = Random.Range(0f, 1f);
        }
    }

    // Ustawienie genów rêcznie (do testów)
    public void SetGenes(float[] newGenes) {
        genes = new float[newGenes.Length];
        for (int i = 0; i < newGenes.Length; i++) {
            genes[i] = newGenes[i];
        }
    }
}
