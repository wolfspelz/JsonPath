using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace JsonPath.Test
{
    [TestClass]
    public class JsonPathNodeXmlTest
    {
        [TestMethod]
        [TestCategory("JsonPath")]
        public void Deserializes_XML()
        {
            // Arrange
            const string data = "<a b='c' d='e'>f<g h='i'>j<k/>l</g>m<n/></a>";

            // Act
            var root = Node.FromXml(data);

            // Assert
            Assert.AreEqual("a", (string)root[Xml.Name]);
            Assert.AreEqual("c", (string)root[Xml.Attributes]["b"]);
            Assert.AreEqual("e", (string)root[Xml.Attributes]["d"]);
            Assert.AreEqual("fm", (string)root[Xml.Text]);
            Assert.AreEqual("f", (string)root[Xml.Children][0]);
            Assert.AreEqual("g", (string)root[Xml.Children][1][Xml.Name]);
            Assert.AreEqual("i", (string)root[Xml.Children][1][Xml.Attributes]["h"]);
            Assert.AreEqual("jl", (string)root[Xml.Children][1][Xml.Text]);
            Assert.AreEqual("k", (string)root[Xml.Children][1][Xml.Children][1][Xml.Name]);
            Assert.AreEqual("m", (string)root[Xml.Children][2]);
            Assert.AreEqual("n", (string)root[Xml.Children][3][Xml.Name]);
        }

        [TestMethod]
        [TestCategory("JsonPath")]
        public void Deserializes_no_text_children()
        {
            // Arrange
            const string data = "<a b='c' d='e'>f<g h='i'>j<k/>l</g>m<n/></a>";

            // Act
            var root = Node.FromXml(data, new XmlDeserializerOptions { TextNodesAsChildren = false });

            // Assert
            Assert.AreEqual("a", (string)root[Xml.Name]);
            Assert.AreEqual("c", (string)root[Xml.Attributes]["b"]);
            Assert.AreEqual("e", (string)root[Xml.Attributes]["d"]);
            Assert.AreEqual("fm", (string)root[Xml.Text]);
            Assert.AreEqual("g", (string)root[Xml.Children][0][Xml.Name]);
            Assert.AreEqual("i", (string)root[Xml.Children][0][Xml.Attributes]["h"]);
            Assert.AreEqual("jl", (string)root[Xml.Children][0][Xml.Text]);
            Assert.AreEqual("k", (string)root[Xml.Children][0][Xml.Children][0][Xml.Name]);
            Assert.AreEqual("n", (string)root[Xml.Children][1][Xml.Name]);
        }

        [TestMethod]
        [TestCategory("JsonPath")]
        public void Deserializes_XML_style_aliases()
        {
            // Arrange
            const string data = "<a b='c' d='e'>f<g h='i'>j<k/>l</g>m<n/></a>";

            // Act
            var root = Node.FromXml(data, new XmlDeserializerOptions { TextNodesAsChildren = false });

            // Assert
            Assert.AreEqual("a", (string)root.Name);
            Assert.AreEqual("c", (string)root.Attributes["b"]);
            Assert.AreEqual("e", (string)root.Attributes["d"]);
            Assert.AreEqual("fm", (string)root.Text);
            Assert.AreEqual("g", (string)root.Children[0].Name);
            Assert.AreEqual("i", (string)root.Children[0].Attributes["h"]);
            Assert.AreEqual("jl", (string)root.Children[0].Text);
            Assert.AreEqual("k", (string)root.Children[0].Children[0].Name);
            Assert.AreEqual("n", (string)root.Children[1].Name);
        }

        [TestMethod]
        [TestCategory("JsonPath")]
        public void Deserializes_Flatten()
        {
            // Arrange
            const string data = "<a b='c' d='e'>f<g h='i'>j<k/>l</g>m<n/></a>";

            // Act
            var root = Node.FromXml(data, new XmlDeserializerOptions { FlattenAttributes = true, TextNodesAsChildren = false });

            // Assert
            Assert.AreEqual("a", (string)root[Xml.Name]);
            Assert.AreEqual("c", (string)root["b"]);
            Assert.AreEqual("e", (string)root["d"]);
            Assert.AreEqual("fm", (string)root[Xml.Text]);
            Assert.AreEqual("g", (string)root[Xml.Children][0][Xml.Name]);
            Assert.AreEqual("i", (string)root[Xml.Children][0]["h"]);
            Assert.AreEqual("jl", (string)root[Xml.Children][0][Xml.Text]);
            Assert.AreEqual("k", (string)root[Xml.Children][0][Xml.Children][0][Xml.Name]);
            Assert.AreEqual("n", (string)root[Xml.Children][1][Xml.Name]);
        }

        [TestMethod]
        [TestCategory("JsonPath")]
        public void Deserializes_Flatten_avoid_collision()
        {
            // Arrange
            const string data = "<a Name='c' d='e'>f<g h='i'>j<k/>l</g>m<n/></a>";
            const string Name = "jhtzftgj6eargdf1";
            const string Children = "jhtzftgj6eargdf2";
            const string Text = "jhtzftgj6eargdf3";

            // Act
            var root = Node.FromXml(data, new XmlDeserializerOptions { Name = Name, Children = Children, Text = Text, FlattenAttributes = true, TextNodesAsChildren = false });

            // Assert
            Assert.AreEqual("a", (string)root[Name]);
            Assert.AreEqual("c", (string)root["Name"]);
            Assert.AreEqual("e", (string)root["d"]);
            Assert.AreEqual("fm", (string)root[Text]);
            Assert.AreEqual("g", (string)root[Children][0][Name]);
            Assert.AreEqual("i", (string)root[Children][0]["h"]);
            Assert.AreEqual("jl", (string)root[Children][0][Text]);
            Assert.AreEqual("k", (string)root[Children][0][Children][0][Name]);
            Assert.AreEqual("n", (string)root[Children][1][Name]);
        }

        [TestMethod]
        [TestCategory("JsonPath")]
        public void Deserializes_avatar_xml()
        {
            // Arrange
            string data = @"
<config xmlns='http://schema.bluehands.de/character-config' version='1.0'>
  <param name='defaultsequence' value='idle'/>
  <sequence group='idle' name='idle' type='status' probability='1000' in='standard' out='standard'><animation src='idle.gif'/></sequence>
  <sequence group='idle' name='idle-1' type='status' probability='50' in='standard' out='standard'><animation src='idle-1.gif'/></sequence>
  <sequence group='idle' name='idle-2' type='status' probability='5' in='standard' out='standard'><animation src='idle-2.gif'/></sequence>
  <sequence group='idle' name='idle-3' type='status' probability='2' in='standard' out='standard'><animation src='idle-3.gif'/></sequence>
  <sequence group='idle' name='idle-4' type='status' probability='1' in='standard' out='standard'><animation src='idle-4.gif'/></sequence>
  <sequence group='moveleft' name='moveleft' type='basic' probability='1' in='moveleft' out='moveleft'><animation dx='-55' dy='0' src='move-l.gif'/></sequence>
  <sequence group='moveright' name='moveright' type='basic' probability='1' in='moveright' out='moveright'><animation dx='55' dy='0' src='move-r.gif'/></sequence>
  <sequence group='chat' name='chat' type='basic' probability='1000' in='standard' out='standard'><animation src='chat.gif'/></sequence>
  <sequence group='chat' name='chat-1' type='basic' probability='100' in='standard' out='standard'><animation src='chat-1.gif'/></sequence>
  <sequence group='wave' name='wave' type='emote' probability='1000' in='standard' out='standard'><animation src='wave.gif'/></sequence>
  <sequence group='kiss' name='kiss' type='emote' probability='1000' in='standard' out='standard'><animation src='kiss.gif'/></sequence>
  <sequence group='strike' name='strike' type='emote' probability='1000' in='standard' out='standard'><animation src='strike.gif'/></sequence>
  <sequence group='cheer' name='cheer' type='emote' probability='1000' in='standard' out='standard'><animation src='cheer.gif'/></sequence>
  <sequence group='deny' name='deny' type='emote' probability='1000' in='standard' out='standard'><animation src='deny.gif'/></sequence>
  <sequence group='clap' name='clap' type='emote' probability='1000' in='standard' out='standard'><animation src='clap.gif'/></sequence>
  <sequence group='dance' name='dance' type='emote' probability='1000' in='standard' out='standard'><animation src='dance.gif'/></sequence>
  <sequence group='yawn' name='yawn' type='emote' probability='1000' in='standard' out='standard'><animation src='yawn.gif'/></sequence>
  <sequence group='angry' name='angry' type='emote' probability='1000' in='standard' out='standard'><animation src='angry.gif'/></sequence>
  <sequence group='laugh' name='laugh' type='emote' probability='1000' in='standard' out='standard'><animation src='laugh.gif'/></sequence>
</config>".Replace("'", "\"");

            // Act
            var root = Node.FromXml(data);

            // Assert
            Assert.AreEqual("config", (string)root[Xml.Name]);
            Assert.AreEqual("http://schema.bluehands.de/character-config", (string)root[Xml.Attributes]["xmlns"]);
            Assert.AreEqual("1.0", (string)root[Xml.Attributes]["version"]);
            Assert.AreEqual("param", (string)root[Xml.Children][0][Xml.Name]);
            Assert.AreEqual("defaultsequence", (string)root[Xml.Children][0][Xml.Attributes]["name"]);
            Assert.AreEqual("idle", (string)root[Xml.Children][0][Xml.Attributes]["value"]);
            Assert.AreEqual("sequence", (string)root[Xml.Children][1][Xml.Name]);
            Assert.AreEqual("idle", (string)root[Xml.Children][1][Xml.Attributes]["group"]);
            Assert.AreEqual("animation", (string)root[Xml.Children][1][Xml.Children][0][Xml.Name]);
            Assert.AreEqual("idle.gif", (string)root[Xml.Children][1][Xml.Children][0][Xml.Attributes]["src"]);

            var names = string.Join("", root.Children.Where(x => x.Name == "sequence").Select(x => x.Attributes["name"].String));
            Assert.AreEqual("idleidle-1idle-2idle-3idle-4moveleftmoverightchatchat-1wavekissstrikecheerdenyclapdanceyawnangrylaugh", names);
        }

    }
}
