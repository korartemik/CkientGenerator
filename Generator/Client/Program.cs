using Generated;

partial class Program
{
    static async Task Main(string[] args)
    {
        Client client = new Client();

        var product1 = new Generated.Object();
        product1.Cost = 100;
        product1.Id = 1;
        product1.Size = 'm';
        _ = client.create(product1);
        var product2 = new Generated.Object();
        product2.Cost = 508;
        product2.Id = 2;
        product2.Size = 'f';
        _ = client.create(product2);

        var ans = await client.getById(1);
        Console.WriteLine(ans.Id + " " + ans.Cost + " " + ans.Size);

        var products = await client.getAll();

        products.ForEach(p =>
        {
            Console.WriteLine(p.Id + " " + p.Cost + " " + p.Size);
        });
    }

}