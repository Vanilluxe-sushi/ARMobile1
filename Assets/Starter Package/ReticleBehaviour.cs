using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ReticleBehaviour : MonoBehaviour
{
    [Header("Dependencias")]
    public DrivingSurfaceManager DrivingSurfaceManager;
    [SerializeField] GameObject Child; // objeto visual del retículo

    // Plano actual bajo el centro de la cámara
    public ARPlane CurrentPlane;

    void Update()
    {
        // 1. Centro de la pantalla en espacio de pantalla (píxeles)
        var screenCenter = Camera.main
            .ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

        // 2. Lanzar rayo contra planos AR detectados
        var hits = new List<ARRaycastHit>();
        DrivingSurfaceManager.RaycastManager.Raycast(
            screenCenter, hits, TrackableType.PlaneWithinBounds);

        // 3. Seleccionar impacto prioritario
        CurrentPlane = null;
        ARRaycastHit? hit = null;

        if (hits.Count > 0)
        {
            var lockedPlane = DrivingSurfaceManager.LockedPlane;

            if (lockedPlane == null)
            {
                // Sin plano fijo → usar el primero detectado
                hit = hits[0];
            }
            else
            {
                // Unity 6: hits.Find() en vez de SingleOrDefault()
                hit = hits.Find(x =>
                    x.trackableId == lockedPlane.trackableId);
            }
        }

        // 4. Mover el retículo al punto de intersección
        if (hit.HasValue)
        {
            CurrentPlane = DrivingSurfaceManager.PlaneManager
                               .GetPlane(hit.Value.trackableId);
            transform.position = hit.Value.pose.position;
        }

        // 5. Visible solo cuando hay plano válido bajo el retículo
        Child.SetActive(CurrentPlane != null);
    }
}