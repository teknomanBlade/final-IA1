using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    //coordenada x en la grilla
    public int x;
    //coordenada y en la grilla
    public int y;
    //G es la distancia recorrida desde el nodo inicial hasta este
    public float g;
    //H es una estimacion de la distancia que me falta para llegar al nodo final
    public float h;
    //F = G + H
    public float f;
    //Referencia al nodo anterior, lo usamos para reconstruir el camino
    public Node previous;
    //Todos los nodos a los que puedo ir desde este
    public List<Node> neighbors = new List<Node>();
    //Radio para detectar cuales nodos estan cerca
    public float radius;
    //Layer de los nodos
    public LayerMask nodesLayer;
    //Si el nodo esta bloqueado o si se puede caminar por el
    public bool isBlocked;
    //Una vez que ya tengo un camino paso esto a true solo para los gizmos
    public bool isPath;



    public void CheckNeighbors()
    {
        //Usamos un overlapSphere para obtener los nodos que tenemos en el radio indicado anteriormente 
        //y filtramos por la layer para no agarrar objetos de mas
        var temp = Physics.OverlapSphere(transform.localPosition, radius, nodesLayer, QueryTriggerInteraction.Collide);
        foreach (var item in temp)
        {
            //Nos aseguramos que no este bloqueado y que no sea el mismo en el que ya estamos
            //y los agregamos a la lista de vecinos
            var node = item.GetComponent<Node>();
            if (node && !node.isBlocked && node != this)
                neighbors.Add(node);
        }
    }

    public void ClearNode()
    {
        //Cada vez que vamos a calcular un camino nuevo reiniciamos los valores de G
        //y del previous para que no interfieran con los nuevos calculos
        g = Mathf.Infinity;
        previous = null;
    }

    private void OnDrawGizmos()
    {
        if (isBlocked)
            Gizmos.color = Color.red;
        else
        {
            // Gizmos.color = Color.blue;
            //Gizmos.DrawWireSphere(transform.position, radious);

        }
        if (isPath)
        {
            Gizmos.color = Color.green;
            if (previous)
                Gizmos.DrawLine(transform.position, previous.transform.position);
        }


        Gizmos.DrawWireSphere(transform.position, radius);

        /*foreach (var n in neighbors)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, n.transform.position);
        }*/

       
    }
    
}
