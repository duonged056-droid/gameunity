using UnityEngine;

namespace DragonBallDefenders
{
    public class WaypointPath : MonoBehaviour
    {
        public Transform[] waypoints;
        public Color pathColor = Color.green;

        public Transform[] GetPoints()
        {
            if (waypoints != null && waypoints.Length > 0)
                return waypoints;

            int childCount = transform.childCount;
            waypoints = new Transform[childCount];
            for (int i = 0; i < childCount; i++)
            {
                waypoints[i] = transform.GetChild(i);
            }
            return waypoints;
        }

        private void OnDrawGizmos()
        {
            Transform[] pts = GetPoints();
            if (pts == null || pts.Length < 2) return;

            Gizmos.color = pathColor;
            for (int i = 0; i < pts.Length - 1; i++)
            {
                if (pts[i] != null && pts[i + 1] != null)
                {
                    Gizmos.DrawLine(pts[i].position, pts[i + 1].position);
                    Gizmos.DrawSphere(pts[i].position, 0.15f);
                }
            }
            if (pts[pts.Length - 1] != null)
            {
                Gizmos.DrawSphere(pts[pts.Length - 1].position, 0.2f);
            }
        }
    }
}
