using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Atom
{
    public int Id;
    public Vector3 Position;
    public string Element;
    //public List<Atom> Neighbours;

    public Atom(int id, Vector3 position, string element)
    {
        Id = id;
        Position = position;
        Element = element;
        //Neighbours = new List<Atom>();
    }

    public void AddNeighbour(Atom neighbour)
    {
        //this.Neighbours.Add(neighbour);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || this.GetType() != obj.GetType())
        {
            return false;
        }

        Atom other = (Atom)obj;
        return other.Id == this.Id;
    }

    public override int GetHashCode()
    {
        return this.Id;
    }

}

public class Bond
{
    public int Id;
    public int From;
    public int To;
    public int BondCount;

    public Bond(int id, int from, int to, int bond_count)
    {
        Id = id;
        From = from;
        To = to;
        BondCount = bond_count;
    }
}

public class Molecule
{

    // 
    public List<Atom> Atoms;
    //public List<Bond> Bonds;

    public Molecule(int atom_count, int bond_count)
    {
        var Atoms = new List<Atom>(atom_count);
        var Bonds = new List<Bond>(bond_count);
    }
}

public class MoleculeMaker : MonoBehaviour
{
    public TextAsset dataFile;
    public List<TextAsset> Files;
    private string[] lines;
    public GameObject polySphere, polyCylinder, secondBond, Sphere;

    /* reads a .txt file into list of lines */ 
    string[] ReadFile(TextAsset txtFile)
    {
        //dataFile = Resources.Load<TextAsset>("C:/User/sanket/Documents/Unity projects/vrProject/Assets/Resources/DB00114parsed.txt");
        var lines = txtFile.text.Split('\n');
        //Debug.Log(dataFile.text);
        //for (int i = 0; i < lines.Length; i++)
        //{
        //    Debug.Log(lines[i]);
        //}
        //Debug.Log("csv");

        return lines;
    }


    // Start is called before the first frame update
    void Start()
    {

        // create molecules from each file 
        for (int fileCount = 0; fileCount < Files.Count; fileCount++)
        {


            /* func call to read the  molecule file*/
            lines = ReadFile(Files[fileCount]);

            
            int noOfBonds, noOfAtoms;
            int.TryParse(lines[0], out noOfAtoms);
            int.TryParse(lines[1], out noOfBonds);
            Debug.Log("no of atoms: " + noOfAtoms);
            Debug.Log("no of bonds: " + noOfBonds);

            float x, y, z = 0.0F;
            int from, to, counts;
            string ele = "-";
            //var M = new Molecule(noOfAtoms, noOfBonds);
            var Atoms = new List<Atom>(noOfAtoms);
            var Bonds = new List<Bond>(noOfBonds);


            int curLine = 2;
            // parse atoms
            for (int i = curLine; i < curLine + noOfAtoms; i++)
            {

                var l = lines[i].Trim().Split('|');
                /* 
                 string s = "";
                 for (int ij = 0; ij < l.Length; ij++)
                     s = s + l[ij].ToString() + '|';
                 Debug.Log(s);
                 */
                //Debug.Log(lines[i]);
                float.TryParse(l[0], out x);
                float.TryParse(l[1], out y);
                float.TryParse(l[2], out z);
                //string.TryParse(lines[i].Split(' ')[3], out ele);
                ele = lines[i].Split('|')[3];
                Atom a = new Atom(i - curLine, new Vector3(x, y, z), ele);
                Atoms.Add(a);
                //Debug.Log(a.Element);
            }

            //Debug.Log(Atoms[8].Element);

            curLine = curLine + noOfAtoms;
            // parse bonds
            for (int i = curLine; i < curLine + noOfBonds; i++)
            {

                var l = lines[i].Trim().Split('|');

                //string s = "";
                //for (int ij = 0; ij < l.Length; ij++)
                //    s = s + l[ij].ToString() + '|';
                //Debug.Log(s);

                //Debug.Log(lines[i]);
                int.TryParse(l[0], out from);
                int.TryParse(l[1], out to);
                int.TryParse(l[2], out counts);
                //string.TryParse(lines[i].Split(' ')[3], out ele);

                Bond b = new Bond(i - curLine, from, to, counts);
                Bonds.Add(b);

            }
            //Debug.Log(Bonds[25].To);

            // func call to render molecule
            GameObject molecule = RenderMolecule(Atoms, Bonds);

            // right now its creating molecules at random positions // later on position will be read thru file.
            molecule.transform.position = new Vector3(Random.Range(-10,10), Random.Range(-10, 10), Random.Range(-10, 10));

        }

    }

    // color's the atom
    void ColorMe(GameObject G, Atom A)
    {
        /* Heres a template we gonna use
         * Properties
            black	bonds/ so we cant use this.
            blue	Phosphorus
            clear	
            cyan	Nitrogen
            gray	Hydrogen / who cares
            green	Oxygen
            magenta	
            red	    Carbon / more focused
            white	
            yellow	Sulphur
        */
        string ele = A.Element;
        if (strCompare(ele, "H")) G.GetComponent<Renderer>().material.color = Color.grey;
        if (strCompare(ele, "C")) G.GetComponent<Renderer>().material.color = Color.red;
        if (strCompare(ele, "O")) G.GetComponent<Renderer>().material.color = Color.green;
        if (strCompare(ele, "S")) G.GetComponent<Renderer>().material.color = Color.yellow;
        if (strCompare(ele, "P")) G.GetComponent<Renderer>().material.color = Color.blue;
        if (strCompare(ele, "N")) G.GetComponent<Renderer>().material.color = Color.cyan;

    }

    // compares string because existing  method is shit
    bool strCompare(string a, string b)
    {
        int size = a.Length < b.Length ? a.Length : b.Length;

        for (int i = 0; i < size; i++)
        {
            if (a[i] != b[i]) return false;

        }

        return true;
    }

    GameObject RenderMolecule (List<Atom> Atoms, List<Bond> Bonds)
    {
        // this is  a parent gameobj that includes all atoms and bonds
        GameObject molecule = new GameObject("Molecule" + Random.Range(0,100).ToString());
        
        
        var Balls = new List<GameObject>(Atoms.Count);
        var Sticks = new List<GameObject>(Bonds.Count);
        var Hydrogens = new List<GameObject>();
        var Carbons = new List<GameObject>();
        var Nitrogens = new List<GameObject>();
        var Phorphoruses = new List<GameObject>();
        var Sulphurs = new List<GameObject>();
        var Oxygens = new List<GameObject>();
        
        
        //Use StringComparer  
        System.StringComparer comparer = System.StringComparer.OrdinalIgnoreCase;


        // draw atoms
        for (int a = 0; a < Atoms.Count; a++)
        {
            GameObject sphere;
            string ele = Atoms[a].Element;
            Debug.Log("ele".GetHashCode());
            
            // carbon as cube // but dont knw mght go against convention
            //if (strCompare(ele,"C"))
            //{
            //    sphere = Instantiate<GameObject>(Cube);
            //    Debug.Log("C deetcted");
            //}
            //else
            
            //{
            //    sphere = Instantiate<GameObject>(polySphere);
            //}

            sphere = Instantiate<GameObject>(polySphere);
            Balls.Add(sphere);
            //GameObject sphere =  GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = Atoms[a].Position;
            sphere.transform.SetParent(molecule.transform,false);
            // func call to color atom
            ColorMe(sphere, Atoms[a]);
            
        }

        
        // draw bonds
        for (int b = 0; b < Bonds.Count; b++)
        {
            var atomIndex1 = Bonds[b].From - 1;
            var atomIndex2 = Bonds[b].To - 1;

            var from = Atoms[atomIndex1].Position;
            var to = Atoms[atomIndex2].Position;

            //draw bonds

                GameObject cylin1 = Instantiate<GameObject>(polyCylinder);
                cylin1.GetComponent<Renderer>().material.color = Color.black;
                //Debug.Log(Vector3.Distance(from, to));
                cylin1.transform.position = (to - from) / 2.0f + from;

                var scale = cylin1.transform.localScale;
                var bondWidth = 6.0f;

                scale.x = bondWidth;
                scale.y = bondWidth;
                scale.z = (to - from).magnitude * 32;

                cylin1.transform.localScale = scale;
                cylin1.transform.rotation = Quaternion.FromToRotation(Vector3.up, to - from);
                cylin1.transform.Rotate(90, 0, 0);
                cylin1.transform.SetParent(molecule.transform, false);
            
            // renders second bond if exist
            if (Bonds[b].BondCount == 2)
            {
                
                GameObject cylin2 = Instantiate<GameObject>(secondBond);
                cylin2.GetComponent<Renderer>().material.color = Color.black;
                //Debug.Log(Vector3.Distance(from, to));
                var offset = cylin2.transform.up;
                cylin2.transform.position = (to - from) / 2.0f + from + offset * 0.15f;
                scale = cylin2.transform.localScale;
                bondWidth = 6.0f;
                
                scale.x = bondWidth;
                scale.y = bondWidth;
                scale.z = (to - from).magnitude * 32;


                cylin2.transform.localScale = scale;
                cylin2.transform.rotation = Quaternion.FromToRotation(Vector3.up, to - from);
                cylin2.transform.Rotate(90, 0, 0);
                cylin2.transform.SetParent(molecule.transform, false);
                cylin1.transform.position -= offset * 0.15f;
            }


            

        }

        // use later
        //molecule.transform.position = new Vector3(0,0,-15);

        return molecule;
    }

    void TestPrimitives()
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0, 5f, 0);

        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = new Vector3(0, 1.5f, 0);

        GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        capsule.transform.position = new Vector3(20, 1, -2);

        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.position = new Vector3(-2, 1, 0);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
