
public static class RandomNetworkTool
{
    private static bool m_isCreatedUser = false;
    private static string m_userName;

    public static string GetRandomUser(int length = 5)
    {
        if (!m_isCreatedUser)
        {
            System.Random random = new System.Random();
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            char[] name = new char[length];
            for (int i = 0; i < length; i++)
            {
                name[i] = chars[random.Next(chars.Length)];
            }
        
            m_userName = new string(name);
            m_isCreatedUser = true;
        
            return m_userName;
        }
        else
        {
            return m_userName;
        }
    }
}
