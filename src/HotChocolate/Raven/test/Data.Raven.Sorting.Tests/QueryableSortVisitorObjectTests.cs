using HotChocolate.Execution;

namespace HotChocolate.Data.Sorting.Expressions;

[Collection(SchemaCacheCollectionFixture.DefinitionName)]
public class QueryableSortVisitorObjectTests
{
    private static readonly Bar[] s_barEntities =
    [
        new()
        {
            Foo = new Foo
            {
                BarShort = 12,
                BarBool = true,
                BarEnum = BarEnum.BAR,
                BarString = "testatest",
                ObjectArray = [new() { Foo = new Foo { BarShort = 12, BarString = "a" } }]
            }
        },
        new()
        {
            Foo = new Foo
            {
                BarShort = 14,
                BarBool = true,
                BarEnum = BarEnum.BAZ,
                BarString = "testbtest",
                //ScalarArray = new[] { "c", "d", "b" },
                ObjectArray =
                [
                    new()
                    {
                        Foo = new Foo
                        {
                            //ScalarArray = new[] { "c", "d", "b" }
                            BarShort = 14, BarString = "d"
                        }
                    }
                ]
            }
        },
        new()
        {
            Foo = new Foo
            {
                BarShort = 13,
                BarBool = false,
                BarEnum = BarEnum.FOO,
                BarString = "testctest",
                //ScalarArray = null,
                ObjectArray = null
            }
        }
    ];

    private static readonly BarNullable[] s_barNullableEntities =
    [
        new()
        {
            Foo = new FooNullable
            {
                BarShort = 12,
                BarBool = true,
                BarEnum = BarEnum.BAR,
                BarString = "testatest",
                //ScalarArray = new[] { "c", "d", "a" },
                ObjectArray =
                [
                    new()
                    {
                        Foo = new FooNullable
                        {
                            //ScalarArray = new[] { "c", "d", "a" }
                            BarShort = 12
                        }
                    }
                ]
            }
        },
        new()
        {
            Foo = new FooNullable
            {
                BarShort = null,
                BarBool = null,
                BarEnum = BarEnum.BAZ,
                BarString = "testbtest",
                //ScalarArray = new[] { "c", "d", "b" },
                ObjectArray =
                [
                    new()
                    {
                        Foo = new FooNullable
                        {
                            //ScalarArray = new[] { "c", "d", "b" }
                            BarShort = null
                        }
                    }
                ]
            }
        },
        new()
        {
            Foo = new FooNullable
            {
                BarShort = 14,
                BarBool = false,
                BarEnum = BarEnum.QUX,
                BarString = "testctest",
                //ScalarArray = null,
                ObjectArray =
                [
                    new()
                    {
                        Foo = new FooNullable
                        {
                            //ScalarArray = new[] { "c", "d", "b" }
                            BarShort = 14
                        }
                    }
                ]
            }
        },
        new()
        {
            Foo = new FooNullable
            {
                BarShort = 13,
                BarBool = false,
                BarEnum = BarEnum.FOO,
                BarString = "testdtest",
                //ScalarArray = null,
                ObjectArray = null
            }
        },
        new() { Foo = null }
    ];

    private readonly SchemaCache _cache;

    public QueryableSortVisitorObjectTests(SchemaCache cache)
    {
        _cache = cache;
    }

    [Fact]
    public async Task Create_ObjectShort_OrderBy()
    {
        // arrange
        var tester = _cache.CreateSchema<Bar, BarSortType>(s_barEntities);

        // act
        var res1 = await tester.ExecuteAsync(
            OperationRequestBuilder.New()
                .SetDocument(
                    "{ root(order: { foo: { barShort: ASC}}) "
                    + "{ foo{ barShort}}}")
                .Build());

        var res2 = await tester.ExecuteAsync(
            OperationRequestBuilder.New()
                .SetDocument(
                    "{ root(order: { foo: { barShort: DESC}}) "
                    + "{ foo{ barShort}}}")
                .Build());

        // assert
        await Snapshot
            .Create()
            .AddResult(res1, "ASC")
            .AddResult(res2, "DESC")
            .MatchAsync();
    }

    [Fact]
    public async Task Create_ObjectNullableShort_OrderBy()
    {
        // arrange
        var tester =
            _cache.CreateSchema<BarNullable, BarNullableSortType>(s_barNullableEntities);

        // act
        var res1 = await tester.ExecuteAsync(
            OperationRequestBuilder.New()
                .SetDocument(
                    "{ root(order: { foo: { barShort: ASC}}) "
                    + "{ foo{ barShort}}}")
                .Build());

        var res2 = await tester.ExecuteAsync(
            OperationRequestBuilder.New()
                .SetDocument(
                    "{ root(order: { foo: { barShort: DESC}}) "
                    + "{ foo{ barShort}}}")
                .Build());

        // assert
        await Snapshot
            .Create()
            .AddResult(res1, "ASC")
            .AddResult(res2, "13")
            .MatchAsync();
    }

    [Fact]
    public async Task Create_ObjectEnum_OrderBy()
    {
        // arrange
        var tester = _cache.CreateSchema<Bar, BarSortType>(s_barEntities);

        // act
        var res1 = await tester.ExecuteAsync(
            OperationRequestBuilder.New()
                .SetDocument(
                    "{ root(order: { foo: { barEnum: ASC}}) "
                    + "{ foo{ barEnum}}}")
                .Build());

        var res2 = await tester.ExecuteAsync(
            OperationRequestBuilder.New()
                .SetDocument(
                    "{ root(order: { foo: { barEnum: DESC}}) "
                    + "{ foo{ barEnum}}}")
                .Build());

        // assert
        await Snapshot
            .Create()
            .AddResult(res1, "ASC")
            .AddResult(res2, "DESC")
            .MatchAsync();
    }

    [Fact]
    public async Task Create_ObjectNullableEnum_OrderBy()
    {
        // arrange
        var tester =
            _cache.CreateSchema<BarNullable, BarNullableSortType>(s_barNullableEntities);

        // act
        var res1 = await tester.ExecuteAsync(
            OperationRequestBuilder.New()
                .SetDocument(
                    "{ root(order: { foo: { barEnum: ASC}}) "
                    + "{ foo{ barEnum}}}")
                .Build());

        var res2 = await tester.ExecuteAsync(
            OperationRequestBuilder.New()
                .SetDocument(
                    "{ root(order: { foo: { barEnum: DESC}}) "
                    + "{ foo{ barEnum}}}")
                .Build());

        // assert
        await Snapshot
            .Create()
            .AddResult(res1, "ASC")
            .AddResult(res2, "13")
            .MatchAsync();
    }

    [Fact]
    public async Task Create_ObjectString_OrderBy()
    {
        // arrange
        var tester = _cache.CreateSchema<Bar, BarSortType>(s_barEntities);

        // act
        var res1 = await tester.ExecuteAsync(
            OperationRequestBuilder.New()
                .SetDocument(
                    "{ root(order: { foo: { barString: ASC}}) "
                    + "{ foo{ barString}}}")
                .Build());

        var res2 = await tester.ExecuteAsync(
            OperationRequestBuilder.New()
                .SetDocument(
                    "{ root(order: { foo: { barString: DESC}}) "
                    + "{ foo{ barString}}}")
                .Build());

        // assert
        await Snapshot
            .Create()
            .AddResult(res1, "ASC")
            .AddResult(res2, "DESC")
            .MatchAsync();
    }

    [Fact]
    public async Task Create_ObjectNullableString_OrderBy()
    {
        // arrange
        var tester =
            _cache.CreateSchema<BarNullable, BarNullableSortType>(s_barNullableEntities);

        // act
        var res1 = await tester.ExecuteAsync(
            OperationRequestBuilder.New()
                .SetDocument(
                    "{ root(order: { foo: { barString: ASC}}) "
                    + "{ foo{ barString}}}")
                .Build());

        var res2 = await tester.ExecuteAsync(
            OperationRequestBuilder.New()
                .SetDocument(
                    "{ root(order: { foo: { barString: DESC}}) "
                    + "{ foo{ barString}}}")
                .Build());

        // assert
        await Snapshot
            .Create()
            .AddResult(res1, "ASC")
            .AddResult(res2, "13")
            .MatchAsync();
    }

    [Fact]
    public async Task Create_ObjectBool_OrderBy()
    {
        // arrange
        var tester = _cache.CreateSchema<Bar, BarSortType>(s_barEntities);

        // act
        var res1 = await tester.ExecuteAsync(
            OperationRequestBuilder.New()
                .SetDocument(
                    "{ root(order: { foo: { barBool: ASC}}) "
                    + "{ foo{ barBool}}}")
                .Build());

        var res2 = await tester.ExecuteAsync(
            OperationRequestBuilder.New()
                .SetDocument(
                    "{ root(order: { foo: { barBool: DESC}}) "
                    + "{ foo{ barBool}}}")
                .Build());

        // assert
        await Snapshot
            .Create()
            .AddResult(res1, "ASC")
            .AddResult(res2, "DESC")
            .MatchAsync();
    }

    [Fact]
    public async Task Create_ObjectNullableBool_OrderBy()
    {
        // arrange
        var tester =
            _cache.CreateSchema<BarNullable, BarNullableSortType>(s_barNullableEntities);

        // act
        var res1 = await tester.ExecuteAsync(
            OperationRequestBuilder.New()
                .SetDocument(
                    "{ root(order: { foo: { barBool: ASC}}) "
                    + "{ foo{ barBool}}}")
                .Build());

        var res2 = await tester.ExecuteAsync(
            OperationRequestBuilder.New()
                .SetDocument(
                    "{ root(order: { foo: { barBool: DESC}}) "
                    + "{ foo{ barBool}}}")
                .Build());

        // assert
        await Snapshot
            .Create()
            .AddResult(res1, "ASC")
            .AddResult(res2, "13")
            .MatchAsync();
    }

    [Fact]
    public async Task Create_ObjectString_OrderBy_TwoProperties()
    {
        // arrange
        var tester = _cache.CreateSchema<Bar, BarSortType>(s_barEntities);

        // act
        var res1 = await tester.ExecuteAsync(
            OperationRequestBuilder.New()
                .SetDocument(
                    "{ root(order: { foo: { barBool: ASC, barShort: ASC }}) "
                    + "{ foo{ barBool barShort}}}")
                .Build());

        var res2 = await tester.ExecuteAsync(
            OperationRequestBuilder.New()
                .SetDocument(
                    @"
                        {
                            root(order: [
                                { foo: { barBool: ASC } },
                                { foo: { barShort: ASC } }]) {
                                foo {
                                    barBool
                                    barShort
                                }
                            }
                        }
                        ")
                .Build());

        var res3 = await tester.ExecuteAsync(
            OperationRequestBuilder.New()
                .SetDocument(
                    "{ root(order: { foo: { barBool: DESC, barShort: DESC}}) "
                    + "{ foo{ barBool barShort}}}")
                .Build());

        var res4 = await tester.ExecuteAsync(
            OperationRequestBuilder.New()
                .SetDocument(
                    @"
                        {
                            root(order: [
                                { foo: { barBool: DESC } },
                                { foo: { barShort: DESC } }]) {
                                foo {
                                    barBool
                                    barShort
                                }
                            }
                        }
                        ")
                .Build());

        // assert
        await Snapshot
            .Create()
            .AddResult(res1, "ASC")
            .AddResult(res2, "ASC")
            .AddResult(res3, "DESC")
            .AddResult(res4, "DESC")
            .MatchAsync();
    }

    [Fact]
    public async Task Create_ObjectString_OrderBy_TwoProperties_Variables()
    {
        // arrange
        var tester = _cache.CreateSchema<Bar, BarSortType>(s_barEntities);

        // act
        var res1 = await tester.ExecuteAsync(
            OperationRequestBuilder.New()
                .SetDocument(
                    @"
                         query testSort($order: [BarSortInput!]) {
                            root(order: $order) {
                                foo {
                                    barBool
                                    barShort
                                }
                            }
                        }")
                .SetVariableValues(
                    new Dictionary<string, object?>
                    {
                        ["order"] = new List<Dictionary<string, object>>
                        {
                            new()
                            {
                                {
                                    "foo",
                                    new Dictionary<string, object>
                                    {
                                        { "barShort", "ASC" }, { "barBool", "ASC" }
                                    }
                                }
                            }
                        }
                    })
                .Build());

        var res2 = await tester.ExecuteAsync(
            OperationRequestBuilder.New()
                .SetDocument(
                    @"
                         query testSort($order: [BarSortInput!]) {
                            root(order: $order) {
                                foo {
                                    barBool
                                    barShort
                                }
                            }
                        }")
                .SetVariableValues(
                    new Dictionary<string, object?>
                    {
                        ["order"] = new List<Dictionary<string, object>>
                        {
                            new()
                            {
                                {
                                    "foo", new Dictionary<string, object> { { "barShort", "ASC" } }
                                }
                            },
                            new()
                            {
                                {
                                    "foo", new Dictionary<string, object> { { "barBool", "ASC" } }
                                }
                            }
                        }
                    })
                .Build());

        var res3 = await tester.ExecuteAsync(
            OperationRequestBuilder.New()
                .SetDocument(
                    @"
                         query testSort($order: [BarSortInput!]) {
                            root(order: $order) {
                                foo {
                                    barBool
                                    barShort
                                }
                            }
                        }")
                .SetVariableValues(
                    new Dictionary<string, object?>
                    {
                        ["order"] = new List<Dictionary<string, object>>
                        {
                            new()
                            {
                                {
                                    "foo",
                                    new Dictionary<string, object>
                                    {
                                        { "barShort", "DESC" }, { "barBool", "DESC" }
                                    }
                                }
                            }
                        }
                    })
                .Build());

        var res4 = await tester.ExecuteAsync(
            OperationRequestBuilder.New()
                .SetDocument(
                    @"
                         query testSort($order: [BarSortInput!]) {
                            root(order: $order) {
                                foo {
                                    barBool
                                    barShort
                                }
                            }
                        }")
                .SetVariableValues(
                    new Dictionary<string, object?>
                    {
                        ["order"] = new List<Dictionary<string, object>>
                        {
                            new()
                            {
                                {
                                    "foo", new Dictionary<string, object> { { "barShort", "DESC" } }
                                }
                            },
                            new()
                            {
                                {
                                    "foo", new Dictionary<string, object> { { "barBool", "DESC" } }
                                }
                            }
                        }
                    })
                .Build());

        // assert
        await Snapshot
            .Create()
            .AddResult(res1, "ASC")
            .AddResult(res2, "ASC")
            .AddResult(res3, "DESC")
            .AddResult(res4, "DESC")
            .MatchAsync();
    }

    public class Foo
    {
        public string? Id { get; set; }

        public short BarShort { get; set; }

        public string BarString { get; set; } = "";

        public BarEnum BarEnum { get; set; }

        public bool BarBool { get; set; }

        //Not supported in SQL
        //public string[] ScalarArray { get; set; }

        public List<Bar>? ObjectArray { get; set; } = [];
    }

    public class FooNullable
    {
        public string? Id { get; set; }

        public short? BarShort { get; set; }

        public string? BarString { get; set; }

        public BarEnum? BarEnum { get; set; }

        public bool? BarBool { get; set; }

        //Not supported in SQL
        //public string?[] ScalarArray { get; set; }

        public List<BarNullable>? ObjectArray { get; set; }
    }

    public class Bar
    {
        public string? Id { get; set; }

        public Foo Foo { get; set; } = null!;
    }

    public class BarNullable
    {
        public string? Id { get; set; }

        public FooNullable? Foo { get; set; }
    }

    public class BarSortType
        : SortInputType<Bar>;

    public class BarNullableSortType
        : SortInputType<BarNullable>;

    public enum BarEnum
    {
        FOO,
        BAR,
        BAZ,
        QUX
    }
}
