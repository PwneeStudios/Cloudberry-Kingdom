using System.Collections.Generic;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class TunnelFill
    {
        /// <summary>
        /// Whether the tunnel has a ceiling.
        /// </summary>
        public bool TunnelCeiling = true;
        public bool TunnelFloor = true;

        public bool RemoveWarts = false;
        public int HeadRoom = 0;
        public int Thickness = 1;

        public ulong[,] TunnelGUIDs;
        public ObjectBase[,] TunnelObjs;

        public TunnelFill()
        {
        }

        int N, M;
        public void Init(int N, int M)
        {
            this.N = N;
            this.M = M;

            TunnelGUIDs = new ulong[N, M];
            TunnelObjs = new ObjectBase[N, M];
        }

        void SetTunnelObjParameter(ObjectBase obj)
        {
            obj.Core.GenData.RemoveIfUnused = false;
        }

        void Clean()
        {
            for (int i = 0; i < N; i++)
                for (int j = 0; j < M; j++)
                    if (TunnelObjs[i, j] != null && TunnelObjs[i, j].Core.MarkedForDeletion)
                        TunnelObjs[i, j] = null;
        }

        void Clean(int i, int j)
        {
            if (TunnelObjs[i, j] == null) return;
            TunnelObjs[i, j].CollectSelf();
            TunnelObjs[i, j] = null;
        }

        public void CleanupTunnel(Level level)
        {
            Dictionary<ulong, ObjectBase> ObjDict = new Dictionary<ulong, ObjectBase>();
            foreach (ObjectBase obj in level.Objects)
                if (!ObjDict.ContainsKey(obj.Core.MyGuid))
                    ObjDict.Add(obj.Core.MyGuid, obj);

            // Convert GUIDs to IObjects
            for (int i = 0; i < N; i++)
                for (int j = 0; j < M; j++)
                {
                    if (ObjDict.ContainsKey(TunnelGUIDs[i, j]))
                        TunnelObjs[i, j] = ObjDict[TunnelGUIDs[i, j]];
                    else
                        TunnelObjs[i, j] = null;
                    //TunnelObjs[i, j] = level.LookupGUID(TunnelGUIDs[i, j]);
                    if (TunnelObjs[i, j] != null && TunnelObjs[i, j].Core.MarkedForDeletion)
                        TunnelObjs[i, j] = null;
                }

            // Head room
            for (int i = 0; i < N; i++)
            {
                for (int j = M - 1; j >= 1; j--)
                {
                    if (TunnelObjs[i, j] == null) continue;
                    if (TunnelObjs[i, j].Core.GenData.Used) continue;

                    if (TunnelObjs[i, j - 1] == null)
                        Clean(i, j);
                }
            }

            // Remove ceiling
            if (!TunnelCeiling)
            {
                for (int i = 0; i < N; i++)
                {
                    int j;
                    for (j = M - 1; j >= 1; j--)
                        if (TunnelObjs[i, j] == null) break;

                    for (; j < M; j++)
                        Clean(i, j);
                }
            }

            // Remove floor
            if (!TunnelFloor)
            {
                for (int i = 0; i < N; i++)
                {
                    int j;
                    for (j = 0; j < M - 1; j++)
                        if (TunnelObjs[i, j] == null) break;

                    for (; j >= 0; j--)
                        Clean(i, j);
                }
            }

            // Remove excess
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    if (TunnelObjs[i, j] == null) continue;
                    if (TunnelObjs[i, j].Core.GenData.Used) continue;

                    if (j - 1 >= 0 && TunnelObjs[i, j - 1] == null) continue;
                    if (j + 1 < M && TunnelObjs[i, j + 1] == null) continue;

                    if (Thickness > 1)
                    {
                        if (j - 2 >= 0 && TunnelObjs[i, j - 2] == null) continue;
                        if (j + 2 < M && TunnelObjs[i, j + 2] == null) continue;
                    }

                    if (Thickness > 2)
                    {
                        if (j - 3 >= 0 && TunnelObjs[i, j - 3] == null) continue;
                        if (j + 3 < M && TunnelObjs[i, j + 3] == null) continue;
                    }

                    TunnelObjs[i, j].CollectSelf();
                }
            }
            Clean();

            // Remove warts
            if (RemoveWarts)
            for (int i = 1; i < N - 1; i++)
            {
                for (int j = 1; j < M; j++)
                {
                    if (TunnelObjs[i, j] == null) continue;
                    if (TunnelObjs[i, j].Core.GenData.Used) continue;

                    if (TunnelObjs[i - 1, j] == null && TunnelObjs[i + 1, j] == null && TunnelObjs[i, j - 1] == null)
                        Clean(i, j);
                }
            }

            TunnelGUIDs = null;
            TunnelObjs = null;
        }
    }
}