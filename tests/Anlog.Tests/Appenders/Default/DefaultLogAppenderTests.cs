﻿using System.Collections.Generic;
using Anlog.Appenders.Default;
using Anlog.Entries;
using Anlog.Formatters.CompactKeyValue;
using Moq;
using Xunit;
using static Anlog.Tests.TestObjects.TestConstants;

namespace Anlog.Tests.Appenders.Default
{
    /// <summary>
    /// Tests for <see cref="DefaultLogAppender"/>
    /// </summary>
    public class DefaultLogAppenderTests
    {
        /// <summary>
        /// Log append data used in tests.
        /// </summary>
        public static IEnumerable<object[]> LogAppendData  =>
            new List<object[]>
            {
                new object[] { "key", null, "key=null" },
                new object[] { "key", string.Empty, "key=" },
                new object[] { TestString.Key, TestString.Value, "string=value" },
                new object[] { TestShort.Key, TestShort.Value, "short=24" },
                new object[] { TestInt.Key, TestInt.Value, "int=69" },
                new object[] { TestLong.Key, TestLong.Value, "long=666" },
                new object[] { TestFloat.Key, TestFloat.Value, "float=24.11" },
                new object[] { TestDouble.Key, TestDouble.Value, "double=69.11" },
                new object[] { TestDecimal.Key, TestDecimal.Value, "decimal=666.11" },
                new object[] { TestDateTime.Key, TestDateTime.Value, "date=2018-03-25 23:00:00.000" },
                new object[] { TestObject.Key, TestObject.Value, "obj={int=24 double=666.11 text=LogTest date=2018-03-25 23:00:00.000 shorts=[1,2,3,4,5]}" },
                new object[] { TestEnum.Key, TestEnum.Value, "enum=Sunday" },
                new object[] { TestArrayInt.Key, TestArrayInt.Value, "arrInt=[11,24,69,666]" },
                new object[] { TestArrayDouble.Key, TestArrayDouble.Value, "arrDouble=[11.1,24.2,69.3,666.4]" },
                new object[] { TestArrayObject.Key, TestArrayObject.Value, "arrObj=[{int=24 double=666.11 text=LogTest date=2018-03-25 23:00:00.000 shorts=[1,2,3,4,5]},{int=24 double=666.11 text=LogTest date=2018-03-25 23:00:00.000 shorts=[1,2,3,4,5]},{int=24 double=666.11 text=LogTest date=2018-03-25 23:00:00.000 shorts=[1,2,3,4,5]}]" },
                new object[] { TestEmptyArray.Key, TestEmptyArray.Value, "emptyArrObj=[]" },
                new object[] { TestEnumerableInt.Key, TestEnumerableInt.Value, "listInt=[11,24,69,666]" },
                new object[] { TestEnumerableDouble.Key, TestEnumerableDouble.Value, "listDouble=[11.1,24.2,69.3,666.4]" },
                new object[] { TestEnumerableObject.Key, TestEnumerableObject.Value, "listObj=[{int=24 double=666.11 text=LogTest date=2018-03-25 23:00:00.000 shorts=[1,2,3,4,5]},{int=24 double=666.11 text=LogTest date=2018-03-25 23:00:00.000 shorts=[1,2,3,4,5]},{int=24 double=666.11 text=LogTest date=2018-03-25 23:00:00.000 shorts=[1,2,3,4,5]}]" },
                new object[] { TestEmptyEnumerable.Key, TestEmptyEnumerable.Value, "emptyListObj=[]" },
                new object[] { null, TestObject.Value, "{int=24 double=666.11 text=LogTest date=2018-03-25 23:00:00.000 shorts=[1,2,3,4,5]}" },
                new object[] { null, TestNonDataContractModelInstance, "{int=69 double=24.11 text=LogTest date=2018-03-25 23:00:00.000 shorts=[1,2,3,4,5]}" },
                new object[] { null, "string", "string" },
                new object[] { null, "", "" }
            };

        [Theory]
        [MemberData(nameof(LogAppendData))]
        public void WhenAppending_WritesCompactKeyValue(string key, object value, string expected)
        {
            string log = null;
            var writer = new Mock<ILogEntriesWriter>();
            writer.Setup(m => m.Write(It.IsAny<LogLevelName>(), It.IsAny<List<ILogEntry>>()))
                .Callback<LogLevelName, List<ILogEntry>>((level, entries) =>
                {
                    var formatter = new CompactKeyValueFormatter(entries);
                    log = formatter.FormatLog(level);
                });
            
            var appender = new DefaultLogAppender(writer.Object, false, "class", "member", 0);
            
            appender.Append(key, value).Info();
            
            Assert.Equal(expected, log.Substring(47));
        }
    }
}