using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genome : MonoBehaviour {
    public float[] genes;
    public int geneLength = 10;

    //[t�yzna, pop�d, zwinno�� , atrakcyjno��, percepcja , si�a]
    // t�yzna wp�ywa na move speed, i zmniejszenie wzrostu g�odu i pragnienia
    // pop�d wp�ywa na to jak cz�sto mo�e si� dany osobnik rozmna�a�, dla samicy oznacza gotowo�� do god�w
    // zwinno�� wp�ywa na pr�dko�� i szans� uniku
    // szansa na akceptacj� samca przez samic�
    // percepcja wp�ywa na zasi�g wykrywania


    private void Start() {
        InitializeRandom();
    }

    // Inicjalizacja genomu losowymi warto�ciami (0-1)
    public void InitializeRandom() {
        genes = new float[geneLength];
        for (int i = 0; i < geneLength; i++) {
            genes[i] = Random.Range(0f, 1f);
        }
    }

    // Ustawienie gen�w r�cznie (do test�w)
    public void SetGenes(float[] newGenes) {
        genes = new float[newGenes.Length];
        for (int i = 0; i < newGenes.Length; i++) {
            genes[i] = newGenes[i];
        }
    }
}
