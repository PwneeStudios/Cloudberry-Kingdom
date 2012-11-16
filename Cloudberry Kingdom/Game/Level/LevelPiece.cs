using System;
using Microsoft.Xna.Framework;


namespace CloudberryKingdom
{
    public class LevelPiece
    {
        public int Par;

        public int StartPhsxStep, DelayStart;

        public Level MyLevel;
        public int NumBobs;
        public Bob[] Computer;

        public int PieceLength;

        public ComputerRecording[] Recording;

        public void Shift(Vector2 shift)
        {
            if (Recording == null) return;

            for (int i = 0; i < Recording.Length; i++)
                if (Recording[i] != null)
                    Recording[i].Shift(shift);
        }

        public PhsxData[] StartData;
        public Vector2[] CheckpointShift;
        public Vector2 CamStartPos;

        public Vector2 LastPoint;

        public PieceSeedData MyData;
        public Level.MakeData MyMakeData;

        public void Release()
        {
            MyData = null;
            MyMakeData = null;

            MyLevel = null;

            StartData = null;

            if (Recording != null)
                foreach (ComputerRecording record in Recording)
                    //ComputerRecording.ToPool(record);
                    record.Release();
            Recording = null;

            if (Computer != null)
                foreach (Bob comp in Computer)
                    comp.Release();

            if (Computer != null)
            for (int i = 0; i < Computer.Length; i++)
                Computer[i].Core.MyLevel = null;
            Computer = null;

            /*
            if (Recording != null)
            for (int i = 0; i < Recording.Length; i++)
                Recording[i].Release();
            Recording = null;*/
            StartData = null;
        }

        public LevelPiece(int Length, Level level, Bob[] computer, int numBobs)
        {
            NumBobs = 0;
            if (computer != null)
                NumBobs = computer.Length;
            else
                NumBobs = numBobs;

            MyLevel = level;
            Computer = computer;

            PieceLength = 6000;// Length;
            int Padding = 100;

            int n = Math.Max(1, NumBobs);
            StartData = new PhsxData[n];
            CheckpointShift = new Vector2[n];
            if (computer != null)
            {
                Recording = new ComputerRecording[NumBobs];                
                for (int i = 0; i < NumBobs; i++)
                {
                    //Recording[i] = new ComputerRecording();
                    //Recording[i].Init(PieceLength + Padding);
                    Recording[i] = ComputerRecording.FromPool();
                    computer[i].MyPieceIndex = i;
                    computer[i].MyRecord = Recording[i];
                }
            }
        }

        public PhsxData GetLastData() { return GetLastData(0); }
        public PhsxData GetLastData(int Index)
        {
            PhsxData LastData = new PhsxData();
            LastData.Position = Recording[Index].AutoLocs[PieceLength - 1];
            LastData.Velocity = Recording[Index].AutoVel[PieceLength - 1];

            return LastData;
        }
    }
}