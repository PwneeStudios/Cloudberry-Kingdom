namespace CloudberryKingdom
{
    public delegate void ScenePartBegin();
    public delegate void ScenePartPhsxStep(int Step);
    public class ScenePart
    {
        public ScenePartBegin MyBegin;
        public ScenePartPhsxStep MyPhsxStep;
        int Step;

        public void PhsxStep()
        {
            if (MyPhsxStep != null)
                MyPhsxStep(Step);
            Step++;
        }

        public void Begin()
        {
            Step = 0;
            if (MyBegin != null)
                MyBegin();
        }
    }
}