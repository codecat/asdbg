int g_frameCount = 0;

int VeryLongFunction()
{
	int ret = 0;
	ret += 1;
	ret += 1;
	ret += 1;
	ret += 1;
	ret += 1;
	ret += 1;
	ret += 1;
	ret += 1;
	ret += 1;
	ret += 1;
	ret += 1;
	ret += 1;
	return ret;
}

string GetString()
{
	string ret = "Hello, world.";
	ret += " " + g_frameCount;
	int test = VeryLongFunction();
	return ret;
}

void main()
{
	string someString = "Hello, world.";
	bool keepGoing = true;

	while (keepGoing) {
		g_frameCount++;

		string formatted = GetString();
		print(formatted);

		sleep(17);
	}

	print("it's the end");
}
