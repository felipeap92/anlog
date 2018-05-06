﻿using System;
using System.Collections.Generic;
using System.Text;
using Anlog.Entries;
using Anlog.Time;

namespace Anlog.Sinks.RollingFile
{
    /// <summary>
    /// Writes the output to files by period and size limit.
    /// <para/>
    /// Files are created using the format log-{date}-{sequence}.txt (log-YYYYMMDD-0001.txt).
    /// </summary>
    public class RollingFileSink : ILogSink, IDisposable
    {
        /// <inheritdoc />
        public LogLevel? MinimumLevel { get; set; }

        /// <inheritdoc />
        public ILogFormatter Formatter { get; }

        /// <summary>
        /// Rolling filer namer.
        /// </summary>
        private RollingFileNamer namer;

        /// <summary>
        /// Renderer factory method.
        /// </summary>
        private Func<IDataRenderer> renderer;

        /// <summary>
        /// File encoding.
        /// </summary>
        private Encoding encoding;
        
        /// <summary>
        /// Buffer size to be used.
        /// </summary>
        private int bufferSize;

        /// <summary>
        /// Internal file sink.
        /// </summary>
        private FileSink sink;

        /// <summary>
        /// Initializes a new instance of <see cref="RollingFileSink"/>.
        /// </summary>
        /// <param name="formatter">Log formatter.</param>
        /// <param name="renderer">Renderer factory method.</param>
        /// <param name="namer">Rolling filer namer.</param>
        /// <param name="encoding">File encoding. The default is UTF8.</param>
        /// <param name="bufferSize">Buffer size to be used. The default is 4096.</param>
        public RollingFileSink(ILogFormatter formatter, Func<IDataRenderer> renderer, RollingFileNamer namer,
            Encoding encoding = null, int bufferSize = 4096)
        {
            Formatter = formatter;
            this.renderer = renderer;
            this.namer = namer;
            this.encoding = encoding;
            this.bufferSize = bufferSize;
            
            CreateSink(TimeProvider.Now);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            sink.Dispose();
        }

        /// <inheritdoc />
        public void Write(LogLevelName level, List<ILogEntry> entries)
        {
            var date = TimeProvider.Now;
            if (namer.ShouldUpdateFile(date))
            {
                CreateSink(date);
            }
            
            sink.Write(level, entries);
        }

        /// <summary>
        /// Creates the sink.
        /// <para/>
        /// If a sink already exists, disposes it.
        /// </summary>
        /// <param name="date">Date to create the sink.</param>
        private void CreateSink(DateTime date)
        {
            sink?.Dispose();
            sink = new FileSink(Formatter, renderer, namer.GetFilePath(date), encoding, bufferSize);
        }
    }
}