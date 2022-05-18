public class PersistentStorage
{
    private int _highscore;

    public bool TryUpdateHighscore(int newScore)
    {
        if (newScore > _highscore)
        {
            _highscore = newScore;
            return true;
        }

        return false;
    }

    public int GetHighscore()
    {
        return _highscore;
    }
}
