class Foo
{
}

void main()
{
	Foo@ f = null;
	int foo = 10;
	int bar = 20;
	string boop = "hello";

	@f = Foo();

	print("Hello, world.");
	print("This is a test!");
	print("This is another test.");

	for (int i = 0; i < 10; i++) {
		print("Number: " + i);
	}
}
