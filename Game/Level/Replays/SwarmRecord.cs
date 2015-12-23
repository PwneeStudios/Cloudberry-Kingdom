using System.Collections.Generic;
using System;
using CloudberryKingdom.Bobs;
using CoreEngine;

namespace CloudberryKingdom.Levels
{
    public class SwarmRecord
    {
        public LevelPiece MyLevelPiece;
        
        public Queue<Recording> Records;
        public Recording MainRecord;
#if XBOX
        int MaxRecords = 200 / Math.Max(1, PlayerManager.GetNumPlayers());
#else
        int MaxRecords = 500 / Math.Max(1, PlayerManager.GetNumPlayers());
#endif
        
        QuadClass BobQuad;

        public void Release()
        {
            MainRecord = null;
            foreach (Recording record in Records)
                record.Release();
            Records = null;
            MyLevelPiece = null;
        }

        public SwarmRecord()
        {
            Records = new Queue<Recording>();

            BobQuad = new QuadClass();
            BobQuad.Base.e1 *= 100;
            BobQuad.Base.e2 *= 100;
            BobQuad.Quad.MyEffect = Tools.BasicEffect;
            BobQuad.Quad.MyTexture = new CoreTexture(); BobQuad.Quad.MyTexture.Name = "BobQuad";
        }

        public void Draw(int Step, Level level, SpriteAnimGroup[] AnimGroup, List<BobLink> BobLinks)
        {
            if (level.SingleOnly)
            {
                MainRecord.Draw(BobQuad, Step, level, AnimGroup, BobLinks);
            }
            else
            {
                foreach (Recording record in Records)
                    record.Draw(BobQuad, Step, level, AnimGroup, BobLinks);
            }
        }

        public void AddRecord(Recording Record, int Step)
        {
            MainRecord = Record;

            if (Records.Count >= MaxRecords)
            {
                Recording DequeuedRecord = Records.Dequeue();
                DequeuedRecord.Release();
            }

			//foreach (Recording record in Records)
			//    if (record != MainRecord)
			//        record.ConvertToSuperSparse(Step);

            Records.Enqueue(Record);
        }
    }
}