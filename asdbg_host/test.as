int g_frameCount = 0;

string GetString()
{
	string ret = "Hello, world.";
	ret += " " + g_frameCount;
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
