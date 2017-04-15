#include <iostream.h>
#include <string>
#include <conio.h>

using namespace std;

static const int charsetsize = 26;
static const int offset = 'a';
static const int maxNum = offset + charsetsize;

//                                  a    b    c    d    e    f    g    h    i    j    k    l    m    n    o    p    q    r    s    t    u    v    w    x    y    z
static int rotor1[charsetsize] = { 'z', 'e', 'x', 'k', 'b', 'h', 'j', 'f', 'm', 'g', 'd', 'r', 'i', 'o', 'n', 'v', 't', 'l', 'u', 'q', 's', 'p', 'y', 'c', 'w', 'a' };
static int rotor2[charsetsize] = { 'c', 'd', 'a', 'b', 'w', 'i', 'p', 'm', 'f', 'k', 'j', 'n', 'h', 'l', 'q', 'g', 'o', 'z', 'u', 'v', 's', 't', 'e', 'y', 'x', 'r' };

static int rotor = 0;

static int map(int input)
{
    int* wiring = rotor1;
    if (rotor == 2)
        wiring = rotor2;
    input -= offset;
    input = wiring[input];
    input += offset;
    return input;
}

static int codec(int c, int& position)
{
    bool bPrint = false;
    if (bPrint) cout << "c         : " << char(c) << ", (" << c << ")" << endl;
    c -= position;
    if (bPrint) cout << "c position: " << char(c) << ", (" << c << ")" << endl;
    if (c < offset)
        c += charsetsize;
    if (bPrint) cout << "c adjusted: " << char(c) << ", (" << c << ")" << endl;
    c = map(c);
    if (bPrint) cout << "c mapped  : " << char(c) << ", (" << c << ")" << endl;
    c += position;
    if (bPrint) cout << "c position: " << char(c) << ", (" << c << ")" << endl;
    if (c >= maxNum)
        c -= charsetsize;
    if (bPrint) cout << "c adjusted: " << char(c) << ", (" << c << ")" << endl;

    // advance rotor
    ++position;
    if (position >= charsetsize)
        position -= charsetsize;

    return c;
}


int main( int argc, const char* argv[] )
{
    for (int i=0; i<charsetsize; ++i)
    {
        // shift the rotors from asci to 0-25
        rotor1[i] -= offset;
        rotor2[i] -= offset;
    }
    for (int i=0; i<charsetsize; ++i)
    {
        // sanity check the rotors
        if (rotor1[rotor1[i]] != i)
            cout << "Error in rotor1, position " << i << ", rotor1[i]: " << rotor1[i] << ", rotor1[rotor1[i]]: " << rotor1[rotor1[i]] << endl;
        if (rotor2[rotor2[i]] != i)
            cout << "Error in rotor2, position " << i << ", rotor2[i]: " << rotor2[i] << ", rotor2[rotor2[i]]: " << rotor2[rotor2[i]] << endl;
    }

    while (rotor != 1 and rotor != 2)
    {
        cout << "Enter rotor (1 or 2): " << endl << endl;
        cin >> rotor;
    }

    char pos = 0;
    while (pos < 'a' || pos > 'z')
    {
        cout << endl << "Enter starting position (lower case letter): " << endl << endl;
        cin >> pos;
    }

    cout << endl << "Enter text (lower case string): " << endl << endl;
    char ch = 0;
    string text;
    while ('\r' != (ch = getch()))
    {
        if (ch >= 'a' && ch <= 'z')
        {
            text.push_back(ch);
            putch(ch);
        }
    }
    text.push_back(0);    
    int startposition = pos - offset;
    int position = startposition;
    
    char* input  = new char[text.size()+1];
    strcpy(input,text.c_str());
    size_t len = strlen(input);
    if (0 == len)
        return 0;
    input[len] = 0;
    cout << endl << endl << "Input: " << input << endl;

    char* output = new char[len+1];
    char* check = new char[len+1];

    position = startposition;

    for (size_t i=0; i<len; ++i)
    {
        output[i] = codec(input[i], position);
    }
    output[len] = 0;

    cout << "Codec: " << output << endl;

    // double check by reversing the string and you should get what you started with
    position = startposition;

    for (size_t i=0; i<len; ++i)
    {
        check[i] = codec(output[i], position);
    }
    check[len] = 0;

    if (strcmp(check, input) != 0)
        cout << "Check failed.  Reverse: " << check << endl;

    return 0;
}

