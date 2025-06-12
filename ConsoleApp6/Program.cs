
using ConsoleApp6;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

int width = 60;
int height = 26;
char[][] board = new char[height][];

SetBoardLines(board);


string TestFile = "massage.json";
string UsersFile = "Users.json";
string IdsFile = "LastId.json";

Dictionary<string, User> users = new();
Dictionary<int, Massages> massages = new();
LastIdInfo lastIdInfo = new();


GetData();

string currentUsername = "";

while (string.IsNullOrWhiteSpace(currentUsername))
{
    IndexPage();

    switch (Console.ReadKey().KeyChar)
    {
        case '1':
            LoginPage();
            break;
        case '2':
            RegisterPage();
            break;
        default:
            Environment.Exit(0);
            break;
    }

    Save();
}
WaitMs();
HomePage();
Save();


// Json
void Save()
{
    string IdsJsonString = JsonConvert.SerializeObject(lastIdInfo, Formatting.Indented);
    File.WriteAllText(IdsFile, IdsJsonString);

    string UsersJsonString = JsonConvert.SerializeObject(users, Formatting.Indented);
    File.WriteAllText(UsersFile, UsersJsonString);

    string MassagesJsonString = JsonConvert.SerializeObject(massages, Formatting.Indented);
    File.WriteAllText(TestFile, MassagesJsonString);
}
void GetData()
{
    string json;

    if (File.Exists(UsersFile))
    {
        json = File.ReadAllText(UsersFile);
        users = JsonConvert.DeserializeObject<Dictionary<string, User>>(json) ?? new();
    }

    if (File.Exists(IdsFile))
    {
        json = File.ReadAllText(IdsFile);
        lastIdInfo = JsonConvert.DeserializeObject<LastIdInfo>(json) ?? new();
    }
    if (File.Exists(TestFile))
    {
        json = File.ReadAllText(TestFile);
        massages = JsonConvert.DeserializeObject<Dictionary<int, Massages>>(json) ?? new();
    }
}

// Start
void IndexPage()
{
    Console.Clear();
    Console.WriteLine("===========");
    Console.WriteLine("1. Login");
    Console.WriteLine("2. Register");
    Console.WriteLine("===========");
    Console.WriteLine("Else to Quit");
}
void LoginPage()
{
    while (string.IsNullOrWhiteSpace(currentUsername))
    {
        Console.Clear();

        Console.Write("| Enter the Username: ");
        string username = Console.ReadLine();

        Console.Write("| Enter the Password: ");
        string password = Console.ReadLine();

        if (username.Length >= 3 && users.ContainsKey(username) && users[username].Password == password)
        {
            currentUsername = username;
            Console.Clear();
            Console.Write($"| Welcome {username} ");
        }
        else
        {
            Console.Clear();
            Console.WriteLine("| Username or Password is invalid, Try Again?");
            Console.WriteLine("| Press 1 to Continue");

            if (Console.ReadKey().KeyChar != '1') break;
        }
    }
}
void RegisterPage()
{

    while (string.IsNullOrWhiteSpace(currentUsername))
    {
        Console.Clear();

        Console.Write("| Enter the Username: ");
        string username = Console.ReadLine();

        Console.Write("| Enter the Password: ");
        string password = Console.ReadLine();

        if (username.Length > 10 || username.Length < 3 || password.Length < 5) continue;


        if (!users.ContainsKey(username))
        {
            currentUsername = username;

            int newId = lastIdInfo.UserID++;
            User newUser = new(newId, currentUsername, password);
            users[currentUsername] = newUser;
        }
        else
        {
            Console.Clear();
            Console.WriteLine("| Username exists!");
            Console.WriteLine("| Press 1 to Continue");

            if (Console.ReadKey().KeyChar != '1')
            {
                break;
            }
        }

    }
}

// Main pages
void HomePage()
{
    while (true)
    {
        string[] Pages = { "My Profile", "Posts", "Friends", "Massages" };

        int curser = 0;
        int start = 0;

        while (true)
        {
            SetBoardDefault();


            char[][] newBoard = SetBoardContents(g1: Pages[0], g4: Pages[1], g7: Pages[2], g10: Pages[3]);

            SetCurser(Pages[curser], curser - start);


            PrintMatrix(newBoard);
            Console.WriteLine("| Press to move");
            Console.WriteLine("| W: Up , S: Down , X: Choose");
            Console.WriteLine("| Else to back home");

            var move = Console.ReadKey();

            if (move.KeyChar == 'w')
            {
                if (curser > 0) curser--;

            }
            else if (move.KeyChar == 's')
            {
                if (curser < Pages.Length - 1) curser++;
            }
            else if (move.KeyChar == 'x')
            {
                switch (curser)
                {
                    case 0:
                        ProfilePage();
                        break;
                    case 1:
                        PostsPage();
                        break;
                    case 2:
                        FriendsPage();
                        break;
                    case 3:
                        MassagesPage();
                        break;
                }
            }
            else return;
        }
    }
}
void ProfilePage()
{
    PrintBoardV1(width, ProfilePageContents());
    Console.WriteLine("Press any Key to Continue");
    Console.ReadKey();

}
void PostsPage()
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("===========");
        Console.WriteLine("1. My Posts");
        Console.WriteLine("2. Add new Post");
        Console.WriteLine("3. Check new Posts");
        Console.WriteLine("===========");
        Console.WriteLine("( Else to Back )");

        switch (Console.ReadKey().KeyChar)
        {
            case '1':
                PrintContents(GetUserPosts());
                break;
            case '2':
                CreatePost();
                break;
            case '3':
                NewPosts(GetNewPosts());
                break;
            default:
                return;
        }
    }

}
void FriendsPage()
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("===========");
        Console.WriteLine($"1. Your friends ({users[currentUsername].Friends.Count})");
        Console.WriteLine("2. Add friends");
        Console.WriteLine($"3. Apply friend requists ({users[currentUsername].FriendRequists.Count})");
        Console.WriteLine("===========");
        Console.WriteLine("( Else to Back )");

        switch (Console.ReadKey().KeyChar)
        {
            case '1':
                switch (users[currentUsername].Friends.Count)
                {
                    case 0:
                        PrintContents(["YOU HAVE NO FRIENDS !!"]);
                        break;
                    default:
                        PrintContents(GetUserFriends());
                        break;
                }
                break;
            case '2':
                SendFriendRequist();
                break;
            case '3':
                ApplyFriendRequsit();
                break;
            default:
                return;
        }
    }
}
void MassagesPage()
{
    List<string> contents = GetUserFriends();

    if (contents.Count == 0)
    {
        Console.Clear();
        Console.WriteLine("| You have no friends to chat with ,poor guy :'(");
        WaitMs();
        return;
    }


    string[] Names = { "", "", "", "" };

    int curser = 0;
    int start = 0;
    int end = 3 >= contents.Count ? contents.Count - 1 : 3;

    while (true)
    {
        SetBoardDefault();

        for (int i = start, x = 0; i <= end; i++, x++)
        {
            Names[x] = $"{i + 1}: {contents[i]}";
        }
        char[][] newBoard = SetBoardContents(g1: Names[0], g4: Names[1], g7: Names[2], g10: Names[3]);

        SetCurser($"{curser + 1}: {contents[curser]}", curser - start);


        PrintMatrix(newBoard);
        Console.WriteLine("| Press to move");
        Console.WriteLine("| W: Up , S: Down , X: Choose");
        Console.WriteLine("| Else to back home");

        var move = Console.ReadKey();

        if (move.KeyChar == 'w')
        {
            if (curser > 0) curser--;

            if (curser < start)
            {
                start--;
                end--;
            }

        }
        else if (move.KeyChar == 's')
        {
            if (curser < contents.Count - 1) curser++;

            if (curser > end)
            {
                start++;
                end++;
            }
        }
        else if (move.KeyChar == 'x')
        {
            int cid = 0;

            foreach (int _cid in users[currentUsername].ChatID)
            {
                if (users[contents[curser]].ChatID.Contains(_cid))
                {
                    cid = _cid;
                }
            }

            FriendChat(cid, contents[curser]);
            Save();
        }
        else return;
    }
}


// Last Posts Page
void NewPosts(List<Post> posts)
{

    if (posts.Count == 0)
    {
        Console.Clear();
        Console.WriteLine("| There is no new posts to show");
        WaitMs();
        return;
    }

    int length = posts.Count;
    int curser = 0;
    int start = 0;

    string g1 = "";
    string g2 = "";
    string g3 = "";
    string g4 = "";
    string g5 = "";
    string g6 = "";
    string g7 = "";
    string g8 = "";
    string g9 = "";
    string g10 = "";
    string g11 = "";
    string g12 = "";

    while (true)
    {
        GetData();
        SetBoardDefault();

        if (start < length)
        {
            string username = posts[start].PosterName == currentUsername ? "You" : posts[start].PosterName;
            g1 = username;
            g2 = $"{posts[start].PostMassage}#h{posts[start].Date.Year}/{posts[start].Date.Month}/{posts[start].Date.Day}#h{posts[start].Date.Hour}:{posts[start].Date.Minute}";

        }
        if (start + 1 < length)
        {
            int current = start + 1;

            string username = posts[current].PosterName == currentUsername ? "You" : posts[current].PosterName;
            g4 = username;
            g5 = $"{posts[current].PostMassage}#h{posts[current].Date.Year}/{posts[current].Date.Month}/{posts[current].Date.Day}#h{posts[current].Date.Hour}:{posts[current].Date.Minute}";
        }
        if (start + 2 < length)
        {
            int current = start + 2;
            string username = posts[current].PosterName == currentUsername ? "You" : posts[current].PosterName;
            g7 = username;
            g8 = $"{posts[current].PostMassage}#h{posts[current].Date.Year}/{posts[current].Date.Month}/{posts[current].Date.Day}#h{posts[current].Date.Hour}:{posts[current].Date.Minute}";
        }
        if (start + 3 < length)
        {
            int current = start + 3;
            string username = posts[current].PosterName == currentUsername ? "You" : posts[current].PosterName;
            g10 = username;
            g11 = $"{posts[current].PostMassage}#h{posts[current].Date.Year}/{posts[current].Date.Month}/{posts[current].Date.Day}#h{posts[current].Date.Hour}:{posts[current].Date.Minute}";
        }

        char[][] newBoard = SetBoardContents(g1, g2, g3, g4, g5, g6, g7, g8, g9, g10, g11, g12);

        string currentPoster = posts[curser].PosterName == currentUsername ? "You" : posts[curser].PosterName;

        SetCurser(currentPoster, curser - start);

        PrintMatrix(newBoard);
        Console.WriteLine("| Press to move");
        Console.WriteLine("| W: Up , S: Down");
        Console.WriteLine("| Else to back home");
        var move = Console.ReadKey();

        if (move.KeyChar == 'w')
        {
            if (curser > 0) curser--;

            if (curser < start) start--;

        }
        else if (move.KeyChar == 's')
        {
            if (curser < posts.Count - 1) curser++;

            if (start + 3 < curser) start++;
        }
        else return;
    }
}
List<Post> GetNewPosts()
{
    PriorityQueue<LinkedListNode<Post>, int> posts = new();

    var firstPost = users[currentUsername].Posts.First;

    if (firstPost != null) posts.Enqueue(firstPost, -firstPost.Value.Id);

    foreach (var friend in users[currentUsername].Friends)
    {
        firstPost = users[friend].Posts.First;

        if (firstPost != null) posts.Enqueue(firstPost, -firstPost.Value.Id);
    }

    List<Post> result = new();

    while (posts.Count > 0 && result.Count < 10)
    {
        var post = posts.Dequeue();

        result.Add(post.Value);

        post = post.Next;

        if (post != null) posts.Enqueue(post, -post.Value.Id);
    }
    return result;
}


// Add New Post
void CreatePost()
{
    Console.Clear();
    Console.Write("| Write your post: ");
    string post = Console.ReadLine();

    if (post.Length < 5)
    {
        Console.Clear();
        Console.WriteLine("| The post should have 5 letters atleast");
        WaitMs();
        return;
    }
    if (post.Length > 40)
    {
        Console.Clear();
        Console.WriteLine("| The post should have less than 40 letters");
        WaitMs();
        return;
    }
    lastIdInfo.PostID++;

    Post newPost = new Post(lastIdInfo.PostID, post, currentUsername, DateTime.Now);
    users[currentUsername].AddPost(newPost);

    Save();
    Console.WriteLine("| Post Added Successfully! ,Press any key to continue");
    Console.ReadKey();

}


// Chat Pages
void FriendChat(int ChatId, string friendname)
{
    GetData();

    int start = massages[ChatId].massagesList.Count - 1;

    string g4 = "";
    string g6 = "";
    string g7 = "";
    string g9 = "";
    string g10 = "";
    string g12 = "";
    while (true)
    {
        GetData();
        SetBoardDefault();

        if (start - 2 >= 0)
        {
            g4 = massages[ChatId].massagesList[start - 2].UserId == users[currentUsername].Id ? massages[ChatId].massagesList[start - 2].MsgString : "";
            g6 = massages[ChatId].massagesList[start - 2].UserId == users[friendname].Id ? massages[ChatId].massagesList[start - 2].MsgString : "";
        }
        if (start - 1 >= 0)
        {
            g7 = massages[ChatId].massagesList[start - 1].UserId == users[currentUsername].Id ? massages[ChatId].massagesList[start - 1].MsgString : "";
            g9 = massages[ChatId].massagesList[start - 1].UserId == users[friendname].Id ? massages[ChatId].massagesList[start - 1].MsgString : "";
        }

        if (start >= 0)
        {
            g10 = massages[ChatId].massagesList[start].UserId == users[currentUsername].Id ? massages[ChatId].massagesList[start].MsgString : "";
            g12 = massages[ChatId].massagesList[start].UserId == users[friendname].Id ? massages[ChatId].massagesList[start].MsgString : "";
        }


        char[][] newBoard = SetBoardContents(g1: "You", g3: friendname, g4: g4, g6: g6, g7: g7, g9: g9, g10: g10, g12: g12);
        SetXLine(newBoard, 4);

        PrintMatrix(newBoard);
        Console.WriteLine("| Press to move");
        Console.WriteLine("| W: Up , S: Down , X: Write new massage");
        Console.WriteLine("| Else to back home");

        var move = Console.ReadKey();

        if (move.KeyChar == 'w')
        {
            if (start - 2 > 0)
            {
                start--;
            }
        }
        else if (move.KeyChar == 's')
        {
            if (start < massages[ChatId].massagesList.Count - 1)
            {
                start++;
            }
        }
        else if (move.KeyChar == 'x')
        {
            AddNewMassage(newBoard, ChatId);
            start = massages[ChatId].massagesList.Count - 1;
        }
        else return;
    }
}
void AddNewMassage(char[][] board, int ChatId)
{
    Console.Clear();
    PrintMatrix(board);
    Console.WriteLine("! Warning: text should be les than 60 letters !");
    Console.Write("| Write the new massage: ");
    string newMassage = Console.ReadLine();

    if (newMassage.Length >= 1 && newMassage.Length <= 60)
    {
        Msg newMsg = new Msg(users[currentUsername].Id, newMassage);
        massages[ChatId].AddMsg(newMsg);
        Save();
    }
}


// Get Contents
List<string> GetUserPosts()
{
    List<string> result = new();
    foreach (var p in users[currentUsername].Posts)
    {
        if (p.PosterName == currentUsername)
        {
            result.Add($"{p.PostMassage}    {p.Date.Year}/{p.Date.Month}/{p.Date.Day}");
        }
    }

    if (result.Count == 0)
    {
        result.Add("| There is no posts");
    }
    return result;
}
List<string> GetUserFriends()
{
    List<string> result = new();

    int i = 1;
    foreach (var friend in users[currentUsername].Friends)
    {
        result.Add(friend);
    }

    return result;
}
List<string> GetUnfriendsUsers()
{
    int i = 1;
    List<string> result = new();
    foreach (var u in users)
    {
        if (u.Value.Name == currentUsername) continue;

        if (!users[currentUsername].Friends.Contains(u.Value.Name) && !u.Value.FriendRequists.Contains(currentUsername) && !users[currentUsername].FriendRequists.Contains(u.Value.Name))
        {
            result.Add(u.Value.Name);
        }
    }

    return result;
}
List<string> GetFriendRequistsUsers()
{
    int i = 1;
    List<string> result = new();
    foreach (var friendName in users[currentUsername].FriendRequists)
    {
        if (users[currentUsername].FriendRequists.Contains(friendName))
        {
            result.Add(friendName);
        }
    }
    return result;
}
List<string> ProfilePageContents()
{
    List<string> result = new();
    Console.Clear();
    result.Add($"| Name: {currentUsername}");
    result.Add($"| Posts count: {users[currentUsername].Posts.Count}");
    result.Add($"| Friends count: {users[currentUsername].Friends.Count}");
    return result;
}


// Friend Set Tools 
void SendFriendRequist()
{
    List<string> contents = GetUnfriendsUsers();

    if (contents.Count == 0)
    {
        Console.Clear();
        Console.WriteLine("| You have no users to show");
        WaitMs();
        return;
    }


    string[] Names = { "", "", "", "" };

    int curser = 0;
    int start = 0;
    int end = 3 >= contents.Count ? contents.Count - 1 : 3;

    while (true)
    {
        SetBoardDefault();

        for (int i = start, x = 0; i <= end; i++, x++)
        {
            Names[x] = $"{i + 1}: {contents[i]}";
        }
        char[][] newBoard = SetBoardContents(g1: Names[0], g4: Names[1], g7: Names[2], g10: Names[3]);

        SetCurser($"{curser + 1}: {contents[curser]}", curser - start);


        PrintMatrix(newBoard);
        Console.WriteLine("| Press to move");
        Console.WriteLine("| W: Up , S: Down , X: Choose");
        Console.WriteLine("| Else to back home");

        var move = Console.ReadKey();

        if (move.KeyChar == 'w')
        {
            if (curser > 0) curser--;

            if (curser < start)
            {
                start--;
                end--;
            }

        }
        else if (move.KeyChar == 's')
        {
            if (curser < contents.Count - 1) curser++;

            if (curser > end)
            {
                start++;
                end++;
            }
        }
        else if (move.KeyChar == 'x')
        {
            users[contents[curser]].AddFriendRequist(currentUsername);
            Save();

            Console.Clear();
            Console.WriteLine("| Friend requist sent successfully");
            WaitMs();
            return;
        }
        else return;
    }
}
void ApplyFriendRequsit()
{
    List<string> contents = GetFriendRequistsUsers();

    if (contents.Count == 0)
    {
        Console.Clear();
        Console.WriteLine("| You have no requists to show");
        WaitMs();
        return;
    }


    string[] Names = { "", "", "", "" };

    int curser = 0;
    int start = 0;
    int end = 3 >= contents.Count ? contents.Count - 1 : 3;

    while (true)
    {
        SetBoardDefault();

        for (int i = start, x = 0; i <= end; i++, x++)
        {
            Names[x] = $"{i + 1}: {contents[i]}";
        }
        char[][] newBoard = SetBoardContents(g1: Names[0], g4: Names[1], g7: Names[2], g10: Names[3]);

        SetCurser($"{curser + 1}: {contents[curser]}", curser - start);


        PrintMatrix(newBoard);
        Console.WriteLine("| Press to move");
        Console.WriteLine("| W: Up , S: Down , X: Choose");
        Console.WriteLine("| Else to back home");

        var move = Console.ReadKey();

        if (move.KeyChar == 'w')
        {
            if (curser > 0) curser--;

            if (curser < start)
            {
                start--;
                end--;
            }

        }
        else if (move.KeyChar == 's')
        {
            if (curser < contents.Count - 1) curser++;

            if (curser > end)
            {
                start++;
                end++;
            }
        }
        else if (move.KeyChar == 'x')
        {
            users[currentUsername].RemoveFriendRequist(contents[curser]);
            users[currentUsername].AddFriend(contents[curser]);
            users[contents[curser]].AddFriend(currentUsername);
            ConnectFriends(contents[curser]);

            Save();
            Console.Clear();
            Console.WriteLine($"| You and {contents[curser]} are friends now");
            return;
        }
        else return;
    }
}
void ConnectFriends(string FriendName)
{
    Random random = new Random();
    int rand = random.Next();

    while (massages.ContainsKey(rand)) rand = random.Next();

    massages[rand] = new Massages(rand);
    users[currentUsername].AddChat(rand);
    users[FriendName].AddChat(rand);
}


// Print Matrix
void PrintMatrix(char[][] board)
{
    Console.Clear();

    for (int h = 0; h < board.Length; h++)
    {
        for (int w = 0; w < width; w++)
        {
            if (h == 0 || h == board.Length - 1)
            {
                if (w == 0 || w == width - 1)
                {
                    Console.Write('*');
                }
                else
                {
                    Console.Write('-');
                }
            }
            else if (w == 0 || w == width - 1)
            {
                Console.Write('|');
            }
            else
            {
                Console.Write(board[h][w]);
            }
        }
        Console.WriteLine();
    }
}


// Set Matrix
string GetGrid(int w, int h, string g1, string g2, string g3, string g4, string g5, string g6, string g7, string g8, string g9, string g10, string g11, string g12)
{
    switch ((w, h))
    {
        case (5, 2):
            return g1;
        case (25, 2):
            return g2;
        case (38, 2):
            return g3;
        case (5, 8):
            return g4;
        case (25, 8):
            return g5;
        case (38, 8):
            return g6;
        case (5, 14):
            return g7;
        case (25, 14):
            return g8;
        case (38, 14):
            return g9;
        case (5, 20):
            return g10;
        case (25, 20):
            return g11;
        case (38, 20):
            return g12;
    }
    return "";
}
char[][] SetBoardContents(string g1 = "", string g2 = "", string g3 = "", string g4 = "", string g5 = "", string g6 = "", string g7 = "", string g8 = "", string g9 = "", string g10 = "", string g11 = "", string g12 = "")
{

    string currentGrid = "";
    for (int h = 0; h < board.Length; h++)
    {
        for (int w = 0; w < width; w++)
        {
            currentGrid = GetGrid(w, h, g1, g2, g3, g4, g5, g6, g7, g8, g9, g10, g11, g12);

            if (currentGrid == "") continue;

            int startW = w;

            int nextH = h;
            int nextW = w + 20;

            for (int x = 0; x < currentGrid.Length; x++, w++)
            {
                if (x < currentGrid.Length - 1 && currentGrid[x] == '#' && currentGrid[x + 1] == 'h')
                {
                    h++;
                    w = startW;
                    x += 2;
                }
                else if (w == nextW)
                {
                    h++;
                    w = startW;

                    if (currentGrid[x] != ' ')
                    {
                        board[h][w] = '-';
                        w++;
                    }
                }

                board[h][w] = currentGrid[x];
            }
            h = nextH;
            w = nextW - 1;

            currentGrid = "";
        }
    }

    return board;
}
void SetCurser(string contents, int pos)
{
    int h = 2;

    switch (pos)
    {
        case 0:
            h = 2;
            break;
        case 1:
            h = 8;
            break;
        case 2:
            h = 14;
            break;
        case 3:
            h = 20;
            break;
    }

    board[h][3] = '>';

    int endContent = 6 + contents.Length > 20 ? 22 : 6 + contents.Length;

    board[h][endContent] = '<';
}
void SetBoardDefault()
{
    for (int h = 0; h < board.Length; h++)
    {
        for (int w = 0; w < width; w++)
        {
            if (h == 0 || h == board.Length - 1)
            {
                if (w == 0 || w == width - 1)
                {
                    board[h][w] = '-';
                }
                else
                {
                    board[h][w] = '-';
                }
            }
            else if (w == 0 || w == width - 1)
            {
                board[h][w] = '|';
            }
            else
            {
                board[h][w] = ' ';
            }
        }
    }
}
void SetBoardLines(char[][] board)
{
    int width = 60;

    for (int i = 0; i < board.Length; i++)
    {
        board[i] = new char[width];
    }
}
void SetXLine(char[][] board, int h)
{
    for (int i = 1; i < board[h].Length - 1; i++)
    {
        board[h][i] = '-';
    }
}


// Print Board (Old)
void PrintBoardV1(int width, List<string> contents, int start = 0)
{
    int height = 15;


    for (int h = 1; h <= height; h++)
    {
        for (int w = 1; w <= width; w++)
        {
            if (h == 1 || h == height)
            {
                if (w == 1 || w == width)
                {
                    Console.Write("*");
                }
                else
                {
                    Console.Write("-");
                }
            }
            else if (w == 1 || w == width)
            {
                Console.Write("|");
            }
            else
            {
                string Content = "";

                if (h % 3 == 0 && w == width / 5)
                {
                    if (start < contents.Count && start >= 0)
                        Content = contents[start++];
                }

                if (string.IsNullOrEmpty(Content))
                {
                    Console.Write(" ");
                }
                else
                {
                    for (int i = 0; i < Content.Length && w < width - 2; i++, w++)
                    {
                        Console.Write(Content[i]);
                    }
                    w--;
                }
            }
        }
        Console.WriteLine();
    }
}
void PrintContents(List<string> contents)
{
    int start = 0;
    while (true)
    {
        Console.Clear();

        PrintBoardV1(width, contents, start);

        Console.WriteLine("    1.Prev  --  2.Next");

        Console.WriteLine("    ( Else to Back )");

        switch (Console.ReadKey().KeyChar)
        {
            case '1':
                start = start - 4 < 0 ? 0 : start - 4;
                break;
            case '2':
                start = start + 4 >= contents.Count ? start : start + 4;
                break;
            default:
                return;
        }
    }
}


//Tread Sleep
void WaitMs()
{
    for (int i = 0; i < 3; i++)
    {
        Console.Write(". ");
        Thread.Sleep(800);
    }
    Thread.Sleep(1500);
}