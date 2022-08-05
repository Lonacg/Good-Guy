using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera[] cameras;
    public Transform target;
    public LayerMask layerMask;
    public float sphereCastRadius=0.5f;
    public Transform debugSphere;

    //Queremos que cambie entre las camaras disponibles si el tarjet esta dentro del campo de vision de la camara
    //Array de camaras ordenado por prioridades
    //Saber si el tarjet es visible
    //Desactivar camaras que no ven al jugador (o hay otra con mayor prioridad que le ve)

    bool IsVisible(Transform target, Camera camera)
    {
        Bounds bounds = new Bounds(target.position,Vector3.zero);
        Plane[] planes=GeometryUtility.CalculateFrustumPlanes(camera);
        bool isInsideFrustrum=GeometryUtility.TestPlanesAABB(planes,bounds);
        if (!isInsideFrustrum) return false;

        Vector3 towardsTarget= (target.transform.position - camera.transform.position).normalized;
        RaycastHit hit;
        Ray ray= new Ray(camera.transform.position, towardsTarget);
        //if (Physics.Raycast(new Ray(camera.transform.position, towardsTarget), out hit, maxDistance: 100, layerMask)) //al poner en unity donde esta el script que en el layer mask no le afecta el cristal, el raycast ya no chocaria con el (otra opcion es ponerle directamente la capa ignoreraycast en la pared)
        if (Physics.SphereCast(ray, sphereCastRadius, out hit, 100, layerMask))
        {
            Debug.DrawLine(camera.transform.position, hit.point, (hit.transform==target) ? Color.green : Color.red); //(hit.transform==target) ? Color.green : Color.red -> es como un if, si se cumple el parenteris entonces color.green, y si no color.red
            debugSphere.position= hit.point;
            debugSphere.localScale= Vector3.one*sphereCastRadius*2;

            return (hit.transform==target);
        }
        return false;

    }






    void Update()
    {
        bool alreadyVisible = false;
        for (int i=0;i<cameras.Length; i++)
        {
            Camera currentCamera=cameras[i];
            if (alreadyVisible)
            {
                currentCamera.enabled=false;
                continue; // en vez de poner continue podemos poner un else y meter todo lo de despues aho
            }
            if (IsVisible(target, currentCamera))
            {
                currentCamera.enabled=true;
                alreadyVisible= true;
            }
            else
            {
                currentCamera.enabled=false;
            }
        }
    }
}





// return sale de la funcion
// break sale del bucle
// continue sale de esa iteracion del bucle y continua en la siguiente