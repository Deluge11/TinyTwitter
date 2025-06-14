
using ConsoleApp6;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Security.Cryptography;

int width = 75;
int height = 26;
char[][] board = new char[height][];

SetBoardLines();


string MassageFile = "massage.json";
string UsersFile = "Users.json";
string IdsFile = "LastId.json";
string PostsFile = "Posts.json";

Dictionary<string, User> users = new();
Dictionary<int, Post> PostsDB = new();
Dictionary<int, Massages> massages = new();
LastIdInfo lastIdInfo = new();


GetData();

//ID
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
    File.WriteAllText(MassageFile, MassagesJsonString);

    string PostsJsonString = JsonConvert.SerializeObject(PostsDB, Formatting.Indented);
    File.WriteAllText(PostsFile, PostsJsonString);
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
    if (File.Exists(MassageFile))
    {
        json = File.ReadAllText(MassageFile);
        massages = JsonConvert.DeserializeObject<Dictionary<int, Massages>>(json) ?? new();
    }
    if (File.Exists(PostsFile))
    {
        json = File.ReadAllText(PostsFile);
        PostsDB = JsonConvert.DeserializeObject<Dictionary<int, Post>>(json) ?? new();
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

        if (username.Length > 10 || username.Length < 3 || password.Length < 5)
        {
            Alert("Username or password is invalid");
            continue;
        }

        if (users.ContainsKey(username))
        {
            Alert("Username already exists");
        }

        currentUsername = username;
        int newId = lastIdInfo.UserID++;
        User newUser = new(newId, currentUsername, password);
        users[currentUsername] = newUser;

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
    string[] Pages = { "My Posts", "Add new Post", "Check new Posts" };

    int curser = 0;
    int start = 0;

    while (true)
    {
        SetBoardDefault();


        char[][] newBoard = SetBoardContents(g2: "#h{ Post Page }", g4: Pages[0], g7: Pages[1], g10: Pages[2]);

        SetCurser(Pages[curser], curser - start + 1);
        SetXLine(6, width - 1, 0);

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
                    PrintContents(GetUserPosts());
                    break;
                case 1:
                    CreatePost();
                    break;
                case 2:
                    NewPosts(GetNewPosts());
                    break;
            }
        }
        else return;
    }

}
void FriendsPage()
{
    while (true)
    {
        string[] Pages = { "My Friends", "Add Friends", "Apply Friend Requist" };

        int curser = 0;
        int start = 0;

        while (true)
        {
            SetBoardDefault();


            char[][] newBoard = SetBoardContents(g2: "#h{ Friend Page }", g4: Pages[0], g7: Pages[1], g10: Pages[2]);

            SetCurser(Pages[curser], curser - start + 1);
            SetXLine(6, width - 1, 0);

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
                        PrintContents(GetUserFriends());
                        break;
                    case 1:
                        SendFriendRequist();
                        break;
                    case 2:
                        ApplyFriendRequsit();
                        break;
                }
            }
            else return;
        }
    }
}
void MassagesPage()
{
    List<string> contents = GetUserFriends();

    if (contents.Count == 0)
    {
        Alert("You have no friends to chat with ,poor guy :'(");
        return;
    }

    int curser = 0;
    int start = 0;

    List<string> friendList = GetUserFriends();


    while (true)
    {
        SetBoardDefault();

        string g4 = "";
        string g7 = "";
        string g10 = "";

        if (friendList.Count - start >= 0)
        {
            g4 = friendList[start + 0];
        }
        if (friendList.Count - start >= 1)
        {
            g7 = friendList[start + 1];
        }
        if (friendList.Count - start >= 2)
        {
            g10 = friendList[start + 2];
        }

        board = SetBoardContents(g2: "#h{ Friend Page }", g4: g4 , g7: g7, g10: g10);
        SetCurser(contents[curser], curser - start + 1);
        SetXLine(6, width - 1, 0);
        PrintMatrix(board);

        Console.WriteLine("| Press to move");
        Console.WriteLine("| W: Up , S: Down , X: Choose");
        Console.WriteLine("| Else to back home");

        var move = Console.ReadKey();

        if (move.KeyChar == 'w')
        {
            if (curser > 0) curser--;

            if (curser < start) start--;

        }
        else if (move.KeyChar == 's')
        {
            if (curser < friendList.Count - 1) curser++;

            if (start + 2 < curser) start++;
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
void NewPosts(List<int> posts)
{

    if (posts.Count == 0)
    {
        Alert("There is no new posts to show");
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
            Post currPost = PostsDB[posts[start]];

            g1 = currPost.PosterName == currentUsername ? "You" : currPost.PosterName;
            g2 = $"{currPost.PostMassage}";
            g3 = $"Like ({currPost.Likes.Count})#hPost in: {currPost.Date.Year}/{currPost.Date.Month}/{currPost.Date.Day}";

        }
        if (start + 1 < length)
        {
            Post currPost = PostsDB[posts[start + 1]];

            g4 = currPost.PosterName == currentUsername ? "You" : currPost.PosterName;
            g5 = $"{currPost.PostMassage}";
            g6 = $"Like ({currPost.Likes.Count})#hPost in: {currPost.Date.Year}/{currPost.Date.Month}/{currPost.Date.Day}";
        }

        if (start + 2 < length)
        {
            Post currPost = PostsDB[posts[start + 2]];

            g7 = currPost.PosterName == currentUsername ? "You" : currPost.PosterName;
            g8 = $"{currPost.PostMassage}";
            g9 = $"Like ({currPost.Likes.Count})#hPost in: {currPost.Date.Year}/{currPost.Date.Month}/{currPost.Date.Day}";
        }
        if (start + 3 < length)
        {
            Post currPost = PostsDB[posts[start + 3]];

            g10 = currPost.PosterName == currentUsername ? "You" : currPost.PosterName;
            g11 = $"{currPost.PostMassage}";
            g12 = $"Like ({currPost.Likes.Count})#hPost in: {currPost.Date.Year}/{currPost.Date.Month}/{currPost.Date.Day}";
        }

        char[][] newBoard = SetBoardContents(g1, g2, g3, g4, g5, g6, g7, g8, g9, g10, g11, g12);

        string currentPoster = PostsDB[posts[curser]].PosterName == currentUsername ? "You" : PostsDB[posts[curser]].PosterName;

        SetCurser(currentPoster, curser - start);

        PrintMatrix(newBoard);
        Console.WriteLine("| Press to move");
        Console.WriteLine("| W: Up , S: Down , X, Like");
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
        else if (move.KeyChar == 'x')
        {
            PostsDB[posts[curser]].Like(currentUsername);
            Save();
        }
        else return;
    }
}
List<int> GetNewPosts()
{
    PriorityQueue<LinkedListNode<int>, int> posts = new();

    var firstPost = users[currentUsername].PostsId.First;

    if (firstPost != null) posts.Enqueue(firstPost, -PostsDB[firstPost.Value].Id);

    foreach (var friend in users[currentUsername].Friends)
    {
        firstPost = users[friend].PostsId.First;

        if (firstPost != null) posts.Enqueue(firstPost, -PostsDB[firstPost.Value].Id);
    }

    List<int> result = new();

    while (posts.Count > 0 && result.Count < 10)
    {
        var post = posts.Dequeue();

        result.Add(PostsDB[post.Value].Id);

        post = post.Next;

        if (post != null) posts.Enqueue(post, -PostsDB[post.Value].Id);
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

    lastIdInfo.PostID++;

    Post newPost = new Post(lastIdInfo.PostID, post, currentUsername, DateTime.Now);
    PostsDB[newPost.Id] = newPost;
    users[currentUsername].AddPost(newPost.Id);

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


        board = SetBoardContents(g1: "      You", g3: friendname, g4: g4, g6: g6, g7: g7, g9: g9, g10: g10, g12: g12);
        SetXLine(4, width - 1);
        SetYLine(width/2,height-1,4);

        PrintMatrix(board);
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
            AddNewMassage(board, ChatId);
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
        Msg newMsg = new Msg(users[currentUsername].Id, newMassage, DateTime.Now);
        massages[ChatId].AddMsg(newMsg);
        Save();
    }
}


// Get Contents
List<string> GetUserPosts()
{
    List<string> result = new();
    foreach (var pId in users[currentUsername].PostsId)
    {
        result.Add($"{PostsDB[pId].PostMassage}    {PostsDB[pId].Date.Year}/{PostsDB[pId].Date.Month}/{PostsDB[pId].Date.Day}");
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
    result.Add($"| Posts count: {users[currentUsername].PostsId.Count}");
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
        Console.WriteLine("| There is no users to show");
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
    List<string> friendRequists = GetFriendRequistsUsers();
    int length = friendRequists.Count - 1;

    if (friendRequists.Count == 0)
    {
        Console.Clear();
        Console.WriteLine("| There is no requists to show");
        WaitMs();
        return;
    }

    int curser = 0;
    int start = 0;


    while (true)
    {
        SetBoardDefault();

        string g1 = "";
        string g4 = "";
        string g7 = "";
        string g10 = "";

        if (length - start >= 0)
        {
            g1 = friendRequists[start + 0];
        }
        if (length - start >= 1)
        {
            g4 = friendRequists[start + 1];
        }
        if (length - start >= 2)
        {
            g7 = friendRequists[start + 2];
        }
        if (length - start >= 3)
        {
            g10 = friendRequists[start + 3];
        }

        board = SetBoardContents(g1:g1, g4: g4, g7: g7, g10: g10);
        SetCurser(friendRequists[curser], curser - start);
        PrintMatrix(board);

        Console.WriteLine("| Press to move");
        Console.WriteLine("| W: Up , S: Down , X: Choose");
        Console.WriteLine("| Else to back home");

        var move = Console.ReadKey();

        if (move.KeyChar == 'w')
        {
            if (curser > 0) curser--;

            if (curser < start) start--;

        }
        else if (move.KeyChar == 's')
        {
            if (curser < friendRequists.Count - 1) curser++;

            if (start + 3 < curser) start++;
        }
        else if (move.KeyChar == 'x')
        {
            users[currentUsername].RemoveFriendRequist(friendRequists[curser]);
            users[currentUsername].AddFriend(friendRequists[curser]);
            users[friendRequists[curser]].AddFriend(currentUsername);
            ConnectFriends(friendRequists[curser]);

            Save();
            Alert($"You and ({friendRequists[curser]}) are friends now");
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
        case (28, 2):
            return g2;
        case (51, 2):
            return g3;
        case (5, 8):
            return g4;
        case (28, 8):
            return g5;
        case (51, 8):
            return g6;
        case (5, 14):
            return g7;
        case (28, 14):
            return g8;
        case (51, 14):
            return g9;
        case (5, 20):
            return g10;
        case (28, 20):
            return g11;
        case (51, 20):
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
            int startH = h;

            int nextW = w + 20;

            for (int x = 0; x < currentGrid.Length; x++, w++)
            {
                if (h - startH == 5) continue;

                if (h - startH == 4 && nextW - w == 4)
                {
                    currentGrid = "...";
                    x = 0;
                }

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
            h = startH;
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

    board[h][3] = '<';

    int endContent = 6 + contents.Length > 25 ? 26 : 6 + contents.Length;

    board[h][endContent] = '>';
}
void SetBoardDefault()
{
    for (int h = 0; h < board.Length; h++)
    {
        for (int w = 0; w < width; w++)
        {
            board[h][w] = ' ';
        }
    }
}
void SetBoardLines()
{
    for (int i = 0; i < board.Length; i++)
    {
        board[i] = new char[width];
    }
}
void SetXLine(int h, int length, int start = 0)
{
    for (int i = start; i < length; i++)
    {
        if (board[h][i] == '|')
        {
            board[h][i] = '*';
        }
        else
        {
            board[h][i] = '-';
        }
    }
}
void SetYLine(int w, int length, int start = 0)
{
    for (int i = start; i < length; i++)
    {
        if (board[i][w] == '-')
        {
            board[i][w] = '*';
        }
        else
        {
            board[i][w] = '|';
        }
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
    if (contents.Count == 0)
    {
        Alert("There is no result to show");
        return;
    }

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
void Alert(string massage)
{
    Console.Clear();
    Console.Write($"| {massage}");
    WaitMs();
}