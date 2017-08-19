void main()
{
	string someString = "Hello, world.";
	int frameCount = 0;
	bool keepGoing = true;

	while (keepGoing) {
		frameCount++;
		print(someString + " " + frameCount);
		sleep(17);
	}

	print("it's the end");
}
