#include <iostream>
#include <map>
#include <string>
#include <fstream>
#include <tuple>

using namespace std;

// function prototype
map<string, tuple<string,string>> GetMap(string filename);
int FindPath(string instructions, map<string, tuple<string,string>> map);

int main() {
    // notes: have a map with the key: string value:Tuple<string,string>

    // get file
    string filename = "PuzzleFile.txt";
    std::ifstream file(filename);
    std::string line;

    // get instructions from the file - they exist in the first line
    string instructions;
    getline(file, instructions);
    
    map<string, tuple<string,string>> map = GetMap(filename);
    int numSteps = FindPath(instructions, map);

    cout << "Number of steps from AAA to ZZZ: " << numSteps <<endl;
    return 0;
}

// define functions
map<string, tuple<string,string>> GetMap(string filename)
{
    std::ifstream file(filename);
    std::string line;
    map<string, tuple<string,string>> map;
    int nodeLength = 3;

    while (getline(file, line))
    {
        // make sure the line in a connections line - can identify with the '='
        if (line.find('=') != string::npos)
        {
            string key = line.substr(0, nodeLength);
            string value1 = line.substr(7, nodeLength);
            string value2 = line.substr(12, nodeLength);

            map[key] = {value1, value2};
        }
    }

    return map;
}

int FindPath(string instructions, map<string, tuple<string,string>> map)
{
    // find start position - AAA
    auto search = map.find("AAA");
    int numSteps = 0;
    int iter = 0;

    while (search->first != "ZZZ")
    {
        if (iter < instructions.size())
        {
            // iterator is smaller than the size of the instructions string so we can use it
            string nextKey = instructions[iter] == 'L'
                ? get<0>(search->second)
                : get<1>(search->second);

            search = map.find(nextKey);
            numSteps++;
            iter ++;
        }
        else
        {
            iter = 0;
        }
    }

    return numSteps;
}